import { Component, output } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-modal-wrapper',
  imports: [
    ReactiveFormsModule
  ],
  template: `
    <div class="modal-overlay" (click)="close.emit()">
      <div class="modal-content bg-dark-gray-800 space-y-4 flex flex-col" (click)="$event.stopPropagation()">
        <ng-content />
      </div>
    </div>
  `,
  styles: `
    .modal-overlay {
      position: fixed;
      z-index: 1;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0, 0, 0, 0.7);
      display: flex;
      justify-content: center;
      align-items: center;
    }

    .modal-content {
      padding: 100px 120px;
      margin: 0 40px;
      border-radius: 10px;
      max-width: 950px;
      width: 100%;
      overflow: hidden;
      height: calc(100% - 144px);
      box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
    }
  `
})
export class ModalWrapperComponent {
  close = output();
}
