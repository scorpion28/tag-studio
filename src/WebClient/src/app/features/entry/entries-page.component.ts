import { Component, inject } from '@angular/core';
import { TopBarComponent } from '../../core/layout/top-bar/top-bar.component';
import { ActivatedRoute, Router } from '@angular/router';
import { EntryService } from './services/entry.service';
import { EntryFormComponent } from './entry-form/entry-form.component';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { Subject } from 'rxjs';
import { CreateEntry, EditEntry } from './models/entry.model';
import { DataTableComponent } from '../../shared/components/data-table.component';

@Component({
  selector: 'app-entries-page',
  standalone: true,
  imports: [
    TopBarComponent,
    EntryFormComponent,
    DataTableComponent,
  ],
  template: `
    <top-bar [pageTitle]="'Entries'" />

    <div class="container w-full my-10">
      <div class="flex justify-between items-center">
        <h2 class="text-alpha-81">Manage Entries</h2>

        <button (click)="selectedEntryId$.next('new')"
                class="text-alpha-81 focus:ring-4 focus:outline-none font-medium rounded-lg text-sm px-3  py-2 text-center inline-flex items-center me-2 my-4 bg-blue-600 hover:bg-blue-700 focus:ring-blue-800">
          <i class="fa-solid fa-plus mr-2 text-2xl mb-1"></i>
          Add Entry
        </button>
      </div>
      @if (this.queryParams()?.['selected']; as entryId) {
        <app-entry-form [entryId]="entryId === 'new' ? '' : entryId" (close)="handleFormOutput($event)" />
      }

      <app-data-table
        [columns]="[
          { header: 'Name', key: 'name' },
          { header: 'Description', key: 'description' }
        ]"
        [items]="entryService.entries()"
        [pagination]="entryService.pagination()"
        [pageSize]="this.entryService.pageSize$.value"
        (edit)="selectedEntryId$.next($event)"
        (remove)="entryService.remove$.next($event)"
        (pageSizeChange)="entryService.pageSize$.next($event)"
        (pageChange)="entryService.page$.next($event)"
      />
    </div>
  `,
})
export class EntriesPageComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  entryService = inject(EntryService);

  selectedEntryId$ = new Subject<string | null>();

  queryParams = toSignal(this.route.queryParams);

  handleFormOutput(entryData: CreateEntry | EditEntry | null) {
    this.selectedEntryId$.next(null);

    if (!entryData) return;
    if ('data' in entryData) {
      this.entryService.edit$.next(entryData);
    } else {
      this.entryService.add$.next(entryData);
    }
  }

  constructor() {
    this.selectedEntryId$
      .pipe(takeUntilDestroyed())
      .subscribe(id =>
        this.router.navigate([], {
          queryParams: { selected: id },
          queryParamsHandling: 'merge',
        }),
      );
  }
}
