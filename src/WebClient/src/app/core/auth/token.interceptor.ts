import { inject } from "@angular/core";
import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from "@angular/common/http";
import { Router } from '@angular/router';
import { catchError, EMPTY, switchMap, throwError } from "rxjs";
import { AuthService } from './services/auth.service';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.includes("/auth")) {
    return next(req);
  }

  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.getTokenSilent()
    .pipe(
      switchMap(token => {
        if (token) {
          const requestWithToken = addTokenToRequest(req, token);
          return next(requestWithToken);
        } else {
          router.navigate(['login']);
          throw EMPTY;
        }
      }),
      catchError(error => {
        if (error instanceof HttpErrorResponse && error.status === 401) {
          console.error('Error retrieving token:', error);
          router.navigate(['/login']);
          return EMPTY;
        }

        return throwError(() => error);
      })
    );
};


function addTokenToRequest(request: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
  return request.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    },
  });
}
