import { Component, input, output } from '@angular/core';
import { NgOptimizedImage } from '@angular/common';
import { EntriesPageMode } from '../entries-page.component';

@Component({
  selector: 'app-entries-page-header',

  template: `
    <div class="flex justify-between items-center">
      <h2 class="text-alpha-81">Manage Entries</h2>

      <div class="text-alpha-81 text-sm flex items-center overflow-hidden">
        <!-- View mode toggle-->
        <div class="flex items-center me-2 outline-1 outline-dark-gray-700 overflow-hidden rounded-lg h-full">
          <div class="p-2 flex items-center" [class.bg-gray-500]="viewMode() === 'table'">
            <button class="rounded-lg m-auto"
                    [disabled]="viewMode() === 'table'"
                    (click)="viewModeSelect.emit('table')">
              <img ngSrc="/list-icon.svg" alt="Table mode" height="20" width="20" />
            </button>
          </div>
          <div class="p-2 flex items-center" [class.bg-gray-500]="viewMode() === 'grid'">
            <button class="rounded-lg"
                    [disabled]="viewMode() === 'grid'"
                    (click)="viewModeSelect.emit('grid')">
              <img ngSrc="/grid-icon.svg" alt="Grid mode" height="20" width="20" />
            </button>
          </div>
        </div>

        <div>
          <button class="text-alpha-81 font-semibold px-3  py-2 focus:ring-4 rounded-lg inline-flex me-2 my-4
            bg-blue-600 hover:bg-blue-700 focus:ring-blue-800"
                  (click)="addEntry.emit()">
            Add Entry
          </button>
        </div>
      </div>
    </div>
  `,
  imports: [
    NgOptimizedImage,
  ],
})
export class EntriesPageHeaderComponent {
  viewMode = input.required<EntriesPageMode>();

  addEntry = output();
  viewModeSelect = output<EntriesPageMode>();
}
