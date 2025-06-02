import { Component, inject, OnInit } from '@angular/core';
import { AsyncPipe, DatePipe } from '@angular/common';
import { TagFormComponent } from '../tag/tag-form/tag-form.component';
import { TopBarComponent } from '../../core/layout/top-bar/top-bar.component';
import { map, Observable, of } from 'rxjs';
import { PaginatedList } from '../../shared/models/paginated-list';
import { ActivatedRoute, Router } from '@angular/router';
import { EntryService } from './services/entry.service';
import { EntryBrief } from './models/entry-brief.model';
import { EntryFormComponent } from './entry-form/entry-form.component';

@Component({
  selector: 'app-entries-page',
  standalone: true,
  imports: [
    AsyncPipe,
    TopBarComponent,
    EntryFormComponent
  ],
  templateUrl: "entries.page.html",
  styles: `
    * {
      font-family: sans-serif;
    }
  `
})
export class EntriesPage implements OnInit {
  private entryService = inject(EntryService);

  paginatedEntries$!: Observable<PaginatedList<EntryBrief>>;
  entries$!: Observable<EntryBrief[]>;
  selectedEntry$!: Observable<'new' | string | undefined>;

  constructor(private route: ActivatedRoute, private router: Router) {

  }

  ngOnInit(): void {
    this.fetchData();
    this.selectedEntry$ = this.route.queryParams
      .pipe(map(params => params['selected']));
  }

  fetchData() {
    this.paginatedEntries$ = this.entryService.getEntries();
    this.entries$ = this.paginatedEntries$.pipe(map(list => list.items));
  }

  onOverviewClose(dataChanged: boolean) {
    this.removeSelectedEntryQueryFromUrl();

    if (dataChanged) {
      this.fetchData();
    }
  }

  editEntry(id: string) {
    this.router.navigate([], {
      queryParams: {
        selected: id
      },
      queryParamsHandling: 'merge',
    });
  }

  removeSelectedEntryQueryFromUrl(): void {
    this.router.navigate([], {
      queryParams: {
        selected: null
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

  removeEntry(id: string) {
    this.entryService.removeEntry(id)
      .subscribe();
    this.fetchData();
  }
}
