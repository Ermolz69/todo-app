import { Component, inject, OnInit, DestroyRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './header.component.html'
})
export class HeaderComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private destroyRef = inject(DestroyRef);

  searchControl = new FormControl('');

  ngOnInit() {
    // Sync initial value from URL
    this.route.queryParams.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
      if (params['search'] !== this.searchControl.value) {
        this.searchControl.setValue(params['search'] || '', { emitEvent: false });
      }
    });

    // Update URL on typing
    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(value => {
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { search: value || null, page: 1 },
        queryParamsHandling: 'merge'
      });
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
