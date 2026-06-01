import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../../core/services/task.service';
import { CategoryService } from '../../../core/services/category.service';
import { Task } from '../../../core/models/task.model';
import { CustomSelectComponent, SelectOption } from '../../../shared/components/custom-select/custom-select.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CustomSelectComponent],
  templateUrl: './task-form.component.html'
})
export class TaskFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private taskService = inject(TaskService);
  categoryService = inject(CategoryService);

  taskForm = this.fb.group({
    title: ['', Validators.required],
    description: [''],
    categoryId: ['', Validators.required]
  });

  isEditing = false;
  editingId: string | null = null;
  currentTask: Task | null = null;
  isOpen = false;
  errorMessage = '';
  onSaved: () => void = () => {};

  categoryOptions: SelectOption[] = [];

  constructor() {
    this.categoryService.categories$.subscribe(categories => {
      this.categoryOptions = categories.map(cat => ({
        label: cat.name,
        value: cat.id
      }));
      // Add default empty option
      this.categoryOptions.unshift({ label: 'Select a category', value: '' });
    });
  }

  ngOnInit() {
    // Ensure categories are loaded for the select dropdown
    this.categoryService.loadCategories();
  }

  openCreate(categoryId?: string, onSavedCallback?: () => void) {
    this.isEditing = false;
    this.editingId = null;
    this.currentTask = null;
    this.errorMessage = '';
    this.taskForm.reset({ title: '', description: '', categoryId: categoryId || '' });
    this.isOpen = true;
    if (onSavedCallback) this.onSaved = onSavedCallback;
  }

  openEdit(task: Task, onSavedCallback?: () => void) {
    this.isEditing = true;
    this.editingId = task.id;
    this.currentTask = task;
    this.errorMessage = '';
    this.taskForm.patchValue({
      title: task.title,
      description: task.description,
      categoryId: task.categoryId
    });
    this.isOpen = true;
    if (onSavedCallback) this.onSaved = onSavedCallback;
  }

  close() {
    this.isOpen = false;
  }

  onSubmit() {
    if (this.taskForm.invalid) return;

    this.errorMessage = '';
    const data = this.taskForm.getRawValue() as any;
    
    // Fix Guid deserialization issue in backend by setting empty string to null
    if (!data.categoryId) {
      data.categoryId = null;
    }
    
    if (this.isEditing && this.editingId) {
      data.isCompleted = this.currentTask?.isCompleted || false;
      this.taskService.updateTask(this.editingId, data).subscribe({
        next: () => {
          this.close();
          this.onSaved();
        },
        error: (err) => {
          this.errorMessage = err?.error?.title || err?.error?.message || 'Failed to update task';
          console.error('Update Task Error', err);
        }
      });
    } else {
      this.taskService.createTask(data).subscribe({
        next: () => {
          this.close();
          this.onSaved();
        },
        error: (err) => {
          this.errorMessage = err?.error?.title || err?.error?.message || 'Failed to create task';
          console.error('Create Task Error', err);
        }
      });
    }
  }
}
