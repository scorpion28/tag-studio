import { Component, computed, inject } from '@angular/core';
import { TagService } from './services/tag.service';
import { FormsModule } from '@angular/forms';
import { TopBarComponent } from '../../core/layout/top-bar/top-bar.component';
import { TagFormComponent } from './tag-form/tag-form.component';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { TagListComponent } from './tag-list.component';
import { CreateTag, EditTag } from './models/tag.model';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-tags-page',
  standalone: true,
  imports: [FormsModule, TopBarComponent, TagFormComponent, DatePipe, TagListComponent],
  templateUrl: './tags-page.component.html',
  styleUrls: ['./tags-page.component.css'],
})
export class TagsPage {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  tagService = inject(TagService);

  queryParams = toSignal(this.route.queryParams);
  selectedTagId$ = new Subject<string | null>();


  handleFormOutput(tagData: CreateTag | EditTag | null): void {
    this.selectedTagId$.next(null);

    if (!tagData) return;
    if ('data' in tagData) {
      this.tagService.edit$.next(tagData);
    } else {
      this.tagService.add$.next(tagData);
    }
  }

  constructor() {
    this.selectedTagId$
      .pipe(takeUntilDestroyed())
      .subscribe(id =>
        this.router.navigate([], {
          queryParams: { selected: id },
          queryParamsHandling: 'merge',
        }),
      );
  }
}
