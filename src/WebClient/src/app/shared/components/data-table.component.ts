import { Component, input, output, TemplateRef } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface TableColumn {
  header: string;
  key: string;
  cellTemplate?: TemplateRef<any>;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  template: `
    <div class=" overflow-clip">
      <table class=" rounded-2xl w-full text-sm text-left  text-gray-500">
        <thead>
          @for (column of columns(); track column.key) {
            <th class="pl-3 py-3">{{ column.header }}</th>
          }
        <th>Actions</th>
        </thead>

        <tbody class="text-alpha-81">
          @for (item of items(); track item.id) {
            <tr class="border-b border-b-dark-gray-650 hover:bg-dark-gray-700">
              @for (column of columns(); track column.key) {
                <td class="pl-2 py-3 border-r border-r-dark-gray-650">
                  @if (column.cellTemplate; as template) {
                    <ng-container
                      [ngTemplateOutlet]="template"
                      [ngTemplateOutletContext]="{ $implicit: item, value: item[column.key], column: column }">
                    </ng-container>
                  } @else {
                    {{ item[column.key] }}
                  }
                </td>
              }
              <td class="px-1">
                <button (click)="edit.emit(item.id)" class="text-blue-500 hover:underline px-1">Edit</button>
                <button (click)="remove.emit(item.id)" class="text-blue-500 hover:underline">Remove</button>
              </td>
            </tr>
          } @empty {
            <tr class="w-full h-12 border-b border-b-dark-gray-400 border-gray-700">
              <td class="text-center" [colSpan]="columns().length + 1">
                There are no items yet
              </td>
            </tr>
          }
        </tbody>
      </table>
    </div>

  `,
  imports: [NgTemplateOutlet, FormsModule],
})
export class DataTableComponent {
  columns = input.required<TableColumn[]>();
  items = input.required<any[]>();

  edit = output<string>();
  remove = output<string>();
}
