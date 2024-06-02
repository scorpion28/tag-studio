import { Component, inject } from '@angular/core';
import { TopBarComponent } from '../../core/layout/top-bar/top-bar.component';
import { ActivatedRoute, Router } from '@angular/router';
import { EntryService } from './services/entry.service';
import { EntryFormComponent } from './entry-form/entry-form.component';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { Subject } from 'rxjs';
import { CreateEntry, EditEntry } from './models/entry.model';

@Component({
  selector: 'app-entries-page',
  standalone: true,
  imports: [
    TopBarComponent,
    EntryFormComponent,
  ],
  templateUrl: 'entries.page.html',
  styles: `
    * {
      font-family: sans-serif;
    }
  `,
})
export class EntriesPage {
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
