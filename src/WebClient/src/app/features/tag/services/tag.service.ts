import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { PaginatedList } from '../../../shared/models/paginated-list';
import { TagDetailed } from '../models/tag-detailed.model';
import { Tag, toTagModel } from '../models/tag.model';

@Injectable({
  providedIn: 'root',
})
export class TagService {
  private baseTagsPath = "api/tags";

  constructor(private http: HttpClient) { }

  getTags(pageNumber: number = 1, pageSize: number = 20): Observable<PaginatedList<Tag>> {
    return this.http.get<PaginatedList<TagDetailed>>(this.baseTagsPath, {
      params: {
        pageNumber: Math.floor(pageNumber),
        pageSize: Math.floor(pageSize)
      }
    })
      .pipe(
        map(list => {
          return {
            ...list,
            items: list.items.map(tagDto => toTagModel(tagDto)),
          };
        })
      );
  }

  getTagById(id: string): Observable<Tag> {
    return this.http.get<TagDetailed>(`${this.baseTagsPath}/${id}`)
      .pipe(
        map(tagDto => toTagModel(tagDto)),
      );
  }

  addTag(newTag: { name: string, parentIds?: string[] }): Observable<Tag> {
    return this.http.post<TagDetailed>(this.baseTagsPath, newTag)
      .pipe(
        map(tagDto => toTagModel(tagDto)),
        catchError(err => {
          console.log(err);
          return throwError(() => err);
        })
      );
  }

  removeTag(id: string): Observable<boolean> {
    return this.http.delete(`${this.baseTagsPath}/${id}`).pipe(
      map(_ => {
        return true;
      }),
      catchError(err => {
        console.log(err);
        return of(false);
      })
    );
  }

  updateTag(id: string, updatedTag: { name: string, parentTagsIds: string[] }): Observable<boolean> {
    return this.http.patch(`${this.baseTagsPath}/${id}`, updatedTag)
      .pipe(
        map(() => true),
        catchError(err => {
          console.log(err);
          return of(false);
        })
      );
  }
}
