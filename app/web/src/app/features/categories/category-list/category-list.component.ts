import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { AsyncPipe, NgClass } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CategoryService } from '../../../core/services/category.service';
import { CategoryFormComponent } from '../category-form/category-form.component';
import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [AsyncPipe, RouterLink, RouterLinkActive, CategoryFormComponent],
  templateUrl: './category-list.component.html'
})
export class CategoryListComponent implements OnInit {
  categoryService = inject(CategoryService);
  
  @ViewChild(CategoryFormComponent) categoryForm!: CategoryFormComponent;

  ngOnInit() {
    this.categoryService.loadCategories();
  }

  openCreateForm() {
    this.categoryForm.openCreate();
  }

  editCategory(category: Category, event: Event) {
    event.preventDefault();
    event.stopPropagation();
    this.categoryForm.openEdit(category);
  }

  deleteCategory(id: string, event: Event) {
    event.preventDefault();
    event.stopPropagation();
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(id).subscribe();
    }
  }
}
