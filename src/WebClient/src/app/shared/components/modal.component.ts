import { Component, contentChild, input, output, TemplateRef } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { NgTemplateOutlet } from '@angular/common';

@Component({
  selector: 'app-modal',
  imports: [
    ReactiveFormsModule,
    NgTemplateOutlet,
  ],
  template: `
    @if (isOpen()) {
      <div class="modal-overlay"
           [style.background-color]="hasBackdrop() ? 'rgba(0, 0, 0, 0.7)' : 'unset'"
           [style.z-index]="layer() ?? 1"
           (click)="close.emit()">
        <div class="{{contentClass()}}" (click)="$event.stopPropagation()">
          <ng-container [ngTemplateOutlet]="template()"></ng-container>
        </div>
      </div>
    }
  `,
  styles: `
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      display: flex;
      justify-content: center;
      align-items: center;
    }

    .default-modal-content {
      color-scheme: dark;
      display: flex;
      flex-direction: column;
      max-width: 950px;
      width: 100%;
      height: calc(100% - 144px);
      margin: 16px 40px;
      padding: 80px 120px;
      border-radius: 10px;
      background-color: var(--color-dark-gray-800);
      overflow-y: auto;
      box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
    }
  `,
})
export class ModalComponent {
  isOpen = input.required<boolean>();
  template = contentChild.required(TemplateRef);
  hasBackdrop = input<boolean>(true);
  layer = input<number>(1);
  contentClass = input<string>('default-modal-content');

  close = output();
}
