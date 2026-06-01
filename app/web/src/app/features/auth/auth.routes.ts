import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { unauthGuard } from '../../core/guards/auth.guard';

export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [unauthGuard]
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [unauthGuard]
  }
];
