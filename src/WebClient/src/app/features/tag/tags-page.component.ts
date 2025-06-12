import { Component, computed, inject } from '@angular/core';
import { TagService } from './services/tag.service';
import { FormsModule } from '@angular/forms';
import { TopBarComponent } from '../../core/layout/top-bar/top-bar.component';
import { TagFormComponent } from './tag-form/tag-form.component';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { TagListComponent } from './tag-list.component';
import { CreateTag, EditTag } from './models/tag.model';
import { Subject } from 'rxjs';
import { DataTableComponent } from '../../shared/components/data-table.component';
import { ModalComponent } from '../../shared/components/modal.component';

@Component({
  selector: 'app-tags-page',
  standalone: true,
  imports: [FormsModule, TopBarComponent, TagFormComponent, DatePipe, TagListComponent, DataTableComponent, ModalComponent],
  template: `
    <top-bar [pageTitle]="'Tags'" />

    <div class="container w-full my-10">
      <div class="flex justify-between items-center  text-alpha-81">
        <h2>Manage Tags</h2>

        <button type="button" (click)="selectTag$.next('new')"
                class="focus:ring-4 focus:outline-none font-medium rounded-lg text-sm px-3  py-2 text-center
                inline-flex items-center me-2 my-4 bg-blue-600 hover:bg-blue-700 focus:ring-blue-800">
          <i class="fa-solid fa-plus mr-2 text-2xl mb-1"></i>
          Add Tag
        </button>
      </div>
      <app-modal [isOpen]="!!this.selectedTag()" (close)="tagForm.onSave()">
        <app-tag-form #tagForm
                      [tagId]="this.selectedTag() === 'new' ? '' : this.selectedTag()"
                      (save)="handleFormOutput($event)"
        />
      </app-modal>

      <app-data-table
        [columns]="
        [
          { header: 'Name', key: 'name' },
          { header: 'Parent tags', key: 'parents', cellTemplate: parentTagsTemplate },
          { header: 'Created', key: 'created', cellTemplate: dateTemplate },
          { header: 'Edited', key: 'lastModified', cellTemplate: dateTemplate }
        ]"
        [items]="tagService.tags()"
        [pagination]="tagService.pagination()"
        [pageSize]="tagService.pageSize$.value"
        (edit)="selectTag$.next($event)"
        (remove)="tagService.remove$.next($event)"
        (pageChange)="tagService.page$.next($event)"
        (pageSizeChange)="tagService.pageSize$.next($event)"
      />

      <ng-template #parentTagsTemplate let-tag>
        <div class="flex flex-row overflow-hidden max-w-3xs">
          <app-tag-list [tags]="tag.parents" />
        </div>
      </ng-template>

      <ng-template #dateTemplate let-tagDate="value">
        {{ tagDate | date }}
      </ng-template>
    </div>
  `,
})
export class TagsPageComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  tagService = inject(TagService);

  selectTag$ = new Subject<string | null>();

  private queryParams = toSignal(this.route.queryParams);
  selectedTag = computed(() => this.queryParams()?.['selected'] as string)

  handleFormOutput(tagData: CreateTag | EditTag | null): void {
    this.selectTag$.next(null);

    if (!tagData) return;
    if ('data' in tagData) {
      this.tagService.edit$.next(tagData);
    } else {
      this.tagService.add$.next(tagData);
    }
  }

  constructor() {
    this.selectTag$
      .pipe(takeUntilDestroyed())
      .subscribe(id =>
        this.router.navigate([], {
          queryParams: { selected: id },
          queryParamsHandling: 'merge',
        }),
      );
  }
}
