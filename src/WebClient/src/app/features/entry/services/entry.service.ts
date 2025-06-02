import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { PaginatedList } from '../../../shared/models/paginated-list';
import { Entry, toEntryModel } from '../models/entry.model';
import { EntryBrief } from '../models/entry-brief.model';
import { EntryDetailed } from '../models/entry-detailed.model';

@Injectable({
  providedIn: 'root',
})
export class EntryService {
  private basePath = "api/entries";

  constructor(private http: HttpClient) { }

  getEntries(pageNumber: number = 1, pageSize: number = 40): Observable<PaginatedList<EntryBrief>> {
    return this.http.get<PaginatedList<EntryBrief>>(this.basePath, {
      params: {
        pageNumber: Math.floor(pageNumber),
        pageSize: Math.floor(pageSize)
      }
    });
  }

  getEntryById(id: string): Observable<Entry> {
    return this.http.get<EntryDetailed>(`${this.basePath}/${id}`)
      .pipe(
        map(entryDto => toEntryModel(entryDto)),
      );
  }

  addEntry(newEntry: { name: string, description?: string }): Observable<Entry> {
    return this.http.post<EntryDetailed>(this.basePath, newEntry)
      .pipe(
        map(entryDto => toEntryModel(entryDto)),
        catchError(err => {
          console.log(err);
          return throwError(() => err);
        })
      );
  }

  removeEntry(id: string): Observable<boolean> {
    return this.http.delete(`${this.basePath}/${id}`).pipe(
      map(_ => {
        return true;
      }),
      catchError(err => {
        console.log(err);
        return of(false);
      })
    );
  }

  updateEntry(id: string, updatedTag: { name: string, description?: string }): Observable<boolean> {
    return this.http.patch(`${this.basePath}/${id}`, updatedTag)
      .pipe(
        map(_ => {
          return true;
        }),
        catchError(err => {
          console.log(err);
          return of(false);
        })
      );
  }

  addTagsToEntry(entryId: string, tagIds: string[]) {
    return this.http.post(`${this.basePath}/${entryId}/tags`, { tagIds: tagIds })
      .pipe(
        map(_ => true),
        catchError(err => {
          console.log(err);
          return of(false);
        })
      );
  }

  removeTagFromEntry(entryId: string, tagId: string) {
    return this.http.delete(`${this.basePath}/${entryId}/tags/${tagId}`);
  }
}
