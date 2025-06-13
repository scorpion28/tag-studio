import { Component, computed, inject, signal } from '@angular/core';
import { TopBarComponent } from '../../core/layout/top-bar/top-bar.component';
import { ActivatedRoute, Router } from '@angular/router';
import { EntryService } from './services/entry.service';
import { EntryFormComponent } from './entry-form/entry-form.component';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { Subject } from 'rxjs';
import { CreateEntry, EditEntry } from './models/entry.model';
import { DataTableComponent } from '../../shared/components/data-table.component';
import { ModalComponent } from '../../shared/components/modal.component';
import { EntriesGridComponent } from './entry-grid/entry-grid.component';
import { EntriesPageHeaderComponent } from './ui/entry-page-header.component';

export type EntriesPageMode = 'table' | 'grid';

@Component({
  selector: 'app-entries-page',
  standalone: true,
  imports: [
    TopBarComponent,
    EntryFormComponent,
    DataTableComponent,
    ModalComponent,
    EntriesGridComponent,
    EntriesPageHeaderComponent,
  ],
  template: `
    <top-bar [pageTitle]="'Entries'" />

    <div class="container w-full my-10 px-10">
      <app-entries-page-header
        [viewMode]="viewMode()"
        (addEntry)="selectEntry$.next('new')"
        (viewModeSelect)="viewMode.set($event)" />

      @if (viewMode() === 'grid') {
        <app-entries-grid
          [entries]="entryService.entries()"
          (entrySelected)="selectEntry$.next($event)" />
      } @else {
        <app-data-table
          [columns]="[
          { header: 'Name', key: 'name' },
          { header: 'Description', key: 'description' }
        ]"
          [items]="entryService.entries()"
          [pagination]="entryService.pagination()"
          [pageSize]="this.entryService.pageSize$.value"
          (edit)="selectEntry$.next($event)"
          (remove)="entryService.remove$.next($event)"
          (pageSizeChange)="entryService.pageSize$.next($event)"
          (pageChange)="entryService.page$.next($event)"
        />
      }
    </div>

    <app-modal
      (close)="entryForm.onClose()"
      [isOpen]="!!selectedEntry()">
      <app-entry-form #entryForm
                      [entryId]="selectedEntry() === 'new' ? '' : selectedEntry()"
                      (save)="handleFormOutput($event)" />
    </app-modal>
  `,
})
export class EntriesPageComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  entryService = inject(EntryService);


  private queryParams = toSignal(this.route.queryParams);
  selectedEntry = computed(() => this.queryParams()?.['selected'] as string ?? '');
  viewMode = signal<'grid' | 'table'>('grid');

  selectEntry$ = new Subject<string | null>();

  handleFormOutput(entryData: CreateEntry | EditEntry | null) {
    this.selectEntry$.next(null);

    if (!entryData) return;
    if ('data' in entryData) {
      this.entryService.edit$.next(entryData);
    } else {
      this.entryService.add$.next(entryData);
    }
  }

  constructor() {
    this.selectEntry$
      .pipe(takeUntilDestroyed())
      .subscribe(id =>
        this.router.navigate([], {
          queryParams: { selected: id },
          queryParamsHandling: 'merge',
        }),
      );
  }
}
