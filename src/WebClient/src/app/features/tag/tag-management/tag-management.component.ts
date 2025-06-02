import { Component, inject, OnInit } from '@angular/core';
import { TagService } from '../services/tag.service';
import { FormsModule } from '@angular/forms';
import { TopBarComponent } from '../../../core/layout/top-bar/top-bar.component';
import { TagFormComponent } from '../tag-form/tag-form.component';
import { AsyncPipe, DatePipe } from '@angular/common';
import { map, Observable } from 'rxjs';
import { PaginatedList } from '../../../shared/models/paginated-list';
import { Tag } from '../models/tag.model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-tag-management',
  standalone: true,
  imports: [FormsModule, TopBarComponent, TagFormComponent, DatePipe, AsyncPipe],
  templateUrl: './tag-management.component.html',
  styleUrls: ['./tag-management.component.css'],
})
export class TagManagementComponent implements OnInit {
  private tagService = inject(TagService);

  paginatedTags$!: Observable<PaginatedList<Tag>>;
  tags$!: Observable<Tag[]>;
  selectedTag$!: Observable<'new' | string | undefined>;

  constructor(private route: ActivatedRoute, private router: Router) {

  }

  ngOnInit(): void {
    this.fetchData();
    this.selectedTag$ = this.route.queryParams
      .pipe(map(params => params['selected']));
  }

  fetchData() {
    this.paginatedTags$ = this.tagService.getTags();
    this.tags$ = this.paginatedTags$.pipe(map(list => list.items));
  }

  onOverviewClose(dataChanged: boolean) {
    this.removeSelectedTagQueryFromUrl();

    if (dataChanged) {
      this.fetchData();
    }
  }

  editTag(id: string) {
    this.router.navigate([], {
      queryParams: {
        selected: id
      },
      queryParamsHandling: 'merge',
    });
  }

  openOverlayForNewTag(): void {
    this.router.navigate([], {
      queryParams: {
        selected: 'new'
      },
      queryParamsHandling: "merge"
    });
  }

  removeSelectedTagQueryFromUrl(): void {
    this.router.navigate([], {
      queryParams: {
        selected: null
      },
      queryParamsHandling: 'merge',
    });
  }

  removeTag(id: string) {
    this.tagService.removeTag(id)
      .subscribe();
    this.fetchData();
  }
}
