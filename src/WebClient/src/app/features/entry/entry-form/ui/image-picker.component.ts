import { Component, input, output } from '@angular/core';
import { ModalComponent } from '../../../../shared/components/modal.component';

@Component({
  selector: 'app-image-picker',
  template: `
    <app-modal
      [isOpen]="isVisible()"
      [layer]="0"
      [hasBackdrop]="false"
      contentClass="'mx-[20%] p-10'"
      (close)="close.emit()"
    >
      <ng-template>
        <div class="bg-dark-gray-750 p-5 w-96 rounded-md modal-shadow font-semibold">
          <button
            class="w-full text-alpha-81 p-1  border-1 rounded-sm border-solid border-dark-gray-200 hover:bg-dark-gray-700 cursor-pointer"
            (click)="fileInput.click()"> Upload image
          </button>
          <input #fileInput class="hidden" type="file" accept="image/*"
                 (change)="selectImage.emit($event)" />

          <button
            class="w-full p-1 mt-5 text-red-500 border-1 border-dark-gray-200 rounded-sm hover:bg-dark-gray-700 cursor-pointer"
            (click)="removeImage.emit()"> Remove
          </button>

          <p class="text-alpha-46 text-center mt-10 font-medium">The maximum size is 5 MB</p>
        </div>
      </ng-template>
    </app-modal>
  `,
  imports: [
    ModalComponent,
  ],
})
export class ImagePickerComponent {
  isVisible = input.required<boolean>();

  selectImage = output<Event>();
  removeImage = output();
  close = output();
}
