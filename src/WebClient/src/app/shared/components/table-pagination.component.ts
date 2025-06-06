import { Component, computed, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaginationInfo } from '../models/pagination';

@Component({
  selector: 'app-table-pagination',
  imports: [
    FormsModule,
  ],
  template: `
    <!-- Pagination -->
    <div class="flex items-center flex-column flex-wrap md:flex-row justify-between pt-4">

      <!-- Pagination text  -->
      <span class="text-sm font-normal text-gray-500 mb-4 md:mb-0 block w-full md:inline md:w-auto">
        Showing
        <span class="font-semibold text-gray-400">
          {{ firstItemNumber() }}-{{ lastItemNumber() }}
        </span>
        of <span class="font-semibold text-gray-400 ">{{ pagination().totalCount }}</span></span>

      <!-- Page size selector-->
      <div class="inline-flex mt-2 xs:mt-0 items-center">
        <div>
          <label class="text-gray-500">Items per page</label>
          <select #select class="text-alpha-81 bg-dark-gray-900"
                  (change)="pageSizeChange.emit(+select.value)">
            @for (option of pageSizeOptions(); track $index) {
              <option [value]="option">{{ option }}</option>
            }
          </select>
        </div>

        <!-- Pagination buttons -->
        <div class="pl-2">
          <button class="px-3 h-8 text-sm font-medium  rounded-s bg-blue-600 border-gray-700 text-alpha-81 hover:bg-blue-700  disabled:bg-dark-gray-350"
                  (click)="previousPage.emit()"
                  [disabled]="!pagination().hasPreviousPage">
            Prev
          </button>
          <button class="px-3 h-8 text-sm font-medium  border-0 border-s rounded-e bg-blue-600 border-gray-700 text-alpha-81 hover:bg-blue-700 disabled:bg-dark-gray-350"
                  (click)="nextPage.emit()"
                  [disabled]="!pagination().hasNextPage">
            Next
          </button>
        </div>
      </div>
    </div>
  `,
})
export class TablePaginationComponent {
  pagination = input.required<PaginationInfo>();
  pageSize = input(10);
  pageSizeOptions = input<number[]>([10, 20, 50]);
  itemsCount = input.required<number>();

  nextPage = output();
  previousPage = output();
  pageSizeChange = output<number>();

  firstItemNumber = computed(() => (this.pagination().pageNumber - 1) * this.pageSize() + 1);
  lastItemNumber = computed(() => this.firstItemNumber() + this.itemsCount() - 1);
}
