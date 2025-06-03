import { CanActivateFn, RedirectCommand, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    let path = router.parseUrl("/login");
    return new RedirectCommand(path, { skipLocationChange: true });
  }

  return true;
};
