import { Component, Input, OnInit, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Tag } from '../models/tag.model';
import { TagBrief } from '../models/tag-brief.model';
import { TagService } from '../services/tag.service';
import { BehaviorSubject, map, of, switchMap, tap } from 'rxjs';
import { TagPickerComponent } from '../tag-picker/tag-picker.component';
import { ModalWrapperComponent } from '../../../shared/components/modal-wrapper/modal-wrapper';

@Component({
  selector: 'app-tag-form',
  standalone: true,
  imports: [
    FormsModule,
    TagPickerComponent,
    ModalWrapperComponent
  ],
  templateUrl: './tag-form.component.html'
})
export class TagFormComponent implements OnInit {
  @Input() tagToEdit: string = "";
  formClose = output<boolean>();

  private isEditMode: boolean = false;
  private initialTagData?: Tag;

  tagName: string = "";
  tagParents = signal<TagBrief[]>([]);

  constructor(private tagService: TagService) {
  }

  ngOnInit() {
    if (!this.tagToEdit || this.tagToEdit === "new") return;

    this.isEditMode = true;

    this.tagService.getTagById(this.tagToEdit)
      .subscribe(tag => {
        this.initialTagData = tag;
        this.tagName = this.initialTagData.name;
        this.tagParents.set(this.initialTagData.parents);
      });

    this.tagService.getTags()
      .pipe(
        map(list => list.items),
        tap(it => this.possibleParentTags$.next(it)));
  }

  onFormClose() {
    this.onSubmit()
      .subscribe(value => this.formClose.emit(value));
  }

  showTagPicker: boolean = false;
  possibleParentTags$ = new BehaviorSubject<TagBrief[]>([]);

  showPossibleParentTags() {
    this.showTagPicker = true;
  }

  hidePossibleParentTags() {
    this.showTagPicker = false;
  }

  onSubmit() {
    return of(this.isEditMode)
      .pipe(
        switchMap(isEdit => {
            if (isEdit) {
              if (!this.initialTagData) {
                return of(undefined);
              }
              return this.updateTag(this.initialTagData?.id, {
                name: this.tagName,
                parentTagsIds: this.tagParents().map(t => t.id)
              });
            } else {
              if (this.tagName.trim().length === 0) {
                return of(undefined);
              }

              return this.createTag({ name: this.tagName });
            }
          }
        ),
        map(value => !!value)
      );
  }

  createTag(createTagModel: { name: string }) {
    return this.tagService.addTag({ name: createTagModel.name });
  }

  updateTag(id: string, updateTagModel: { name: string, parentTagsIds: string[] }) {
    return this.tagService.updateTag(id, updateTagModel);
  }

  onParentTagAdded(tag: TagBrief) {
    this.tagParents.update(tags => [...tags, tag]);
  }

  onParentTagRemoved(tag: TagBrief) {
    this.tagParents.update(tags => tags.filter(value => value !== tag));
  }
}
