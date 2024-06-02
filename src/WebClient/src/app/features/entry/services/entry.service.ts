import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  catchError,
  concatMap, EMPTY,
  map,
  merge,
  mergeMap,
  Observable,
  of,
  startWith,
  Subject,
  switchMap,
} from 'rxjs';
import { PaginatedList } from '../../../shared/models/paginated-list';
import { CreateEntry, EditEntry, Entry, RemoveEntry, toEntryModel } from '../models/entry.model';
import { EntryBrief } from '../models/entry-brief.model';
import { EntryDetailed } from '../models/entry-detailed.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

interface EntriesState {
  entriesPaginated: PaginatedList<EntryBrief>;
  loaded: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class EntryService {
  private baseUrl = 'api/entries';

  private http = inject(HttpClient);

  private state = signal<EntriesState>({
    entriesPaginated: {
      items: [],
      pageNumber: 1,
      totalPages: 1,
      totalCount: 0,
      hasNextPage: false,
      hasPreviousPage: false,
    },
    loaded: false,
    error: null,
  });

  // selectors
  entries = computed(() => this.state().entriesPaginated.items);

  // sources
  add$ = new Subject<CreateEntry>();
  edit$ = new Subject<EditEntry>();
  remove$ = new Subject<RemoveEntry>();

  entryAdded$ = this.add$.pipe(
    concatMap(addEntry =>
      this.http
        .post(this.baseUrl, addEntry)
        .pipe(catchError(err => this.handleError(err))),
    ),
  );

  entryEdited$ = this.edit$.pipe(
    mergeMap(update =>
      this.http
        .patch(`${this.baseUrl}/${update.id}`, update.data)
        .pipe(catchError(err => this.handleError(err))),
    ),
  );

  entryRemoved$ = this.remove$.pipe(
    mergeMap((id) =>
      this.http
        .delete(`${this.baseUrl}/${id}`)
        .pipe(catchError(err => this.handleError(err))),
    ),
  );

  getEntryById(id: string): Observable<Entry> {
    return this.http.get<EntryDetailed>(`${this.baseUrl}/${id}`)
      .pipe(
        map(entryDto => toEntryModel(entryDto)),
      );
  }

  addTagsToEntry(entryId: string, tagIds: string[]) {
    return this.http.post(`${this.baseUrl}/${entryId}/tags`, { tagIds: tagIds })
      .pipe(
        map(_ => true),
        catchError(err => {
          console.log(err);
          return of(false);
        }),
      );
  }

  removeTagFromEntry(entryId: string, tagId: string) {
    return this.http.delete(`${this.baseUrl}/${entryId}/tags/${tagId}`);
  }

  constructor() {
    // reducers
    merge(this.entryAdded$, this.entryEdited$, this.entryRemoved$)
      .pipe(
        startWith(null),
        switchMap(() =>
          this.http.get<PaginatedList<EntryBrief>>(this.baseUrl, {
            params: {
              pageNumber: Math.floor(1),
              pageSize: Math.floor(50),
            },
          }),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((entriesPaginated) =>
        this.state.update(state => ({
            ...state,
            entriesPaginated: entriesPaginated,
            loaded: true,
          }),
        ),
      );
  }

  private handleError(error: any) {
    this.state.update(state => ({
        ...state,
        error: error,
      }),
    );
    return EMPTY;
  }
}
