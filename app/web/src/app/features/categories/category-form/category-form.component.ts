import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './category-form.component.html'
})
export class CategoryFormComponent {
  private fb = inject(FormBuilder);
  private categoryService = inject(CategoryService);

  categoryForm = this.fb.group({
    name: ['', Validators.required],
    color: ['#4f46e5', Validators.required],
    icon: ['folder', Validators.required] // simplified icon
  });

  isEditing = false;
  editingId: string | null = null;
  isOpen = false;
  errorMessage = '';

  openCreate() {
    this.isEditing = false;
    this.editingId = null;
    this.errorMessage = '';
    this.categoryForm.reset({ name: '', color: '#4f46e5', icon: 'folder' });
    this.isOpen = true;
  }

  openEdit(category: Category) {
    this.isEditing = true;
    this.editingId = category.id;
    this.errorMessage = '';
    this.categoryForm.patchValue({
      name: category.name,
      color: category.color,
      icon: category.icon
    });
    this.isOpen = true;
  }

  close() {
    this.isOpen = false;
  }

  onSubmit() {
    if (this.categoryForm.invalid) return;
    this.errorMessage = '';
    const data = this.categoryForm.getRawValue() as any;
    
    if (this.isEditing && this.editingId) {
      this.categoryService.updateCategory(this.editingId, data).subscribe({
        next: () => this.close(),
        error: (err) => {
          this.errorMessage = err?.error?.title || err?.error?.message || 'Failed to update category';
          console.error('Update Category Error', err);
        }
      });
    } else {
      this.categoryService.createCategory(data).subscribe({
        next: () => this.close(),
        error: (err) => {
          this.errorMessage = err?.error?.title || err?.error?.message || 'Failed to create category';
          console.error('Create Category Error', err);
        }
      });
    }
  }
}
