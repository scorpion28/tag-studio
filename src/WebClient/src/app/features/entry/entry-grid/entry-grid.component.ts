import { Component, input, output } from '@angular/core';
import { EntryCardComponent } from './entry-card.component';
import { Entry } from '../models/entry.model';

@Component({
  selector: 'app-entries-grid',
  template: `
    <div class="flex flex-wrap justify-center gap-5">
      @for (entry of entries(); track $index) {
        <app-entry-card [entry]="entry" (click)="entrySelected.emit(entry.id)" />
      }
    </div>
  `,
  imports: [
    EntryCardComponent,
  ],
})
export class EntriesGridComponent {
  entries = input.required<Entry[]>();

  entrySelected = output<string>();
}
