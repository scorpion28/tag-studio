import { Component, Input, OnInit, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { map, of, switchMap } from 'rxjs';
import { Entry } from '../models/entry.model';
import { EntryService } from '../services/entry.service';
import { TagBrief } from '../../tag/models/tag-brief.model';
import { TagPickerComponent } from '../../tag/tag-picker/tag-picker.component';
import { ModalWrapperComponent } from '../../../shared/components/modal-wrapper/modal-wrapper';

@Component({
  selector: 'app-entry-form',
  standalone: true,
  imports: [
    FormsModule,
    TagPickerComponent,
    ModalWrapperComponent
  ],
  templateUrl: './entry-form.component.html'
})
export class EntryFormComponent implements OnInit {
  @Input() idOfEntry: string = "";
  formClose = output<boolean>();

  private isEditMode: boolean = false;
  private initialEntryData?: Entry;

  name: string = "";
  description: string = "";
  tags = signal<TagBrief[]>([]);

  showTagPicker = false;

  constructor(private entryService: EntryService) {
  }

  ngOnInit() {
    if (this.idOfEntry) {
      this.isEditMode = true;

      this.entryService.getEntryById(this.idOfEntry)
        .subscribe(entry => {
          this.initialEntryData = entry;
          this.name = this.initialEntryData.name;
          this.description = this.initialEntryData.description ?? "";
          this.tags.set(this.initialEntryData.tags);
        });
    }
  }

  closeForm() {
    this.onSubmit()
      .subscribe(value => this.formClose.emit(value));
  }

  onSubmit() {
    return of(this.isEditMode)
      .pipe(
        switchMap(isEdit => {
            if (isEdit) {
              if (!this.initialEntryData) {
                return of(undefined);
              }

              return this.updateEntry(this.initialEntryData?.id, {name: this.name, description: this.description});
            } else {
              if (this.name.trim().length === 0) {
                return of(undefined);
              }
              return this.createEntry({name: this.name, description: this.description})
                .pipe(
                  switchMap(created => {
                    return this.entryService.addTagsToEntry(created.id, this.tags().map(tag => tag.id));
                  })
                );
            }
          }
        ),
        map(value => !!value)
      );
  }

  createEntry(createEntryModel: { name: string, description?: string }) {
    return this.entryService.addEntry({name: createEntryModel.name, description: createEntryModel.description});
  }

  updateEntry(id: string, updateTagModel: { name: string, description?: string }) {
    return this.entryService.updateEntry(id, updateTagModel);
  }

  showPossibleParentTags() {
    this.showTagPicker = true;
  }

  hidePossibleParentTags() {
    this.showTagPicker = false;
  }

  removeEntryTag(tag: TagBrief) {
    this.entryService.removeTagFromEntry(this.initialEntryData?.id!, tag.id)
      .subscribe(_ => this.tags.update(tags => tags.filter(t => t.id !== tag.id)));
  }

  addEntryTag(tag: TagBrief) {
    if (this.isEditMode) {
      this.entryService.addTagsToEntry(this.initialEntryData?.id!, [tag.id])
        .subscribe(_ => this.tags.update(tags => [...tags, tag]));
    } else {
      this.tags.update((tags) => [...tags, tag]);
    }
  }
}
