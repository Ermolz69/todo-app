import { Component, inject, OnInit, ViewChild, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskService } from '../../../core/services/task.service';
import { CategoryService } from '../../../core/services/category.service';
import { TaskFormComponent } from '../task-form/task-form.component';
import { Task } from '../../../core/models/task.model';
import { PaginatedResult } from '../../../core/models/pagination.model';
import { DatePipe, NgClass } from '@angular/common';
import { CustomSelectComponent, SelectOption } from '../../../shared/components/custom-select/custom-select.component';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [TaskFormComponent, DatePipe, NgClass, CustomSelectComponent],
  templateUrl: './task-list.component.html'
})
export class TaskListComponent implements OnInit {
  private taskService = inject(TaskService);
  private categoryService = inject(CategoryService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  @ViewChild(TaskFormComponent) taskForm!: TaskFormComponent;

  tasksData: PaginatedResult<Task> | null = null;
  isLoading = false;

  currentCategoryId: string = '';
  currentCategoryName: string = 'All Tasks';
  currentStatus: string = ''; // '' for all, 'true' for completed, 'false' for active
  currentPage: number = 1;
  pageSize: number = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  pageSizeSelectOptions: SelectOption[] = [
    { label: '5', value: 5 },
    { label: '10', value: 10 },
    { label: '20', value: 20 },
    { label: '50', value: 50 }
  ];

  ngOnInit() {
    // Listen to query params changes to fetch tasks
    this.route.queryParams.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
      this.currentCategoryId = params['categoryId'] || '';
      this.currentStatus = params['isCompleted'] || '';
      this.currentPage = params['page'] ? +params['page'] : 1;
      this.pageSize = params['pageSize'] ? +params['pageSize'] : 10;
      this.updateCategoryName();
      this.loadTasks(params);
    });
    
    // Update category name if categories change
    this.categoryService.categories$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      this.updateCategoryName();
    });
  }

  updateCategoryName() {
    if (!this.currentCategoryId) {
      this.currentCategoryName = 'All Tasks';
      return;
    }
    this.categoryService.categories$.subscribe(categories => {
      const cat = categories.find(c => c.id === this.currentCategoryId);
      if (cat) this.currentCategoryName = cat.name;
    });
  }

  loadTasks(params: any) {
    this.isLoading = true;
    this.taskService.getTasks({
      categoryId: params['categoryId'],
      search: params['search'],
      isCompleted: params['isCompleted'],
      page: params['page'] || 1,
      pageSize: params['pageSize'] || this.pageSize
    }).subscribe({
      next: (data) => {
        this.tasksData = data;
        this.pageSize = data.pageSize;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  refreshTasks() {
    this.loadTasks(this.route.snapshot.queryParams);
  }

  openCreateForm() {
    this.taskForm.openCreate(this.currentCategoryId, () => this.refreshTasks());
  }

  editTask(task: Task) {
    this.taskForm.openEdit(task, () => this.refreshTasks());
  }

  deleteTask(id: string) {
    if (confirm('Are you sure you want to delete this task?')) {
      this.taskService.deleteTask(id).subscribe(() => this.refreshTasks());
    }
  }

  toggleComplete(task: Task) {
    const updatedStatus = !task.isCompleted;
    this.taskService.updateTask(task.id, { 
      title: task.title,
      description: task.description,
      categoryId: task.categoryId,
      isCompleted: updatedStatus 
    }).subscribe(() => this.refreshTasks());
  }

  changeStatusFilter(status: string) {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { isCompleted: status || null, page: 1 },
      queryParamsHandling: 'merge'
    });
  }

  changePage(page: number) {
    if (page < 1 || (this.tasksData && page > Math.ceil(this.tasksData.totalCount / this.pageSize))) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page },
      queryParamsHandling: 'merge'
    });
  }

  changePageSize(size: number) {
    if (size === this.pageSize) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageSize: size, page: 1 },
      queryParamsHandling: 'merge'
    });
  }

  get totalPages(): number {
    return this.tasksData ? Math.ceil(this.tasksData.totalCount / this.pageSize) : 1;
  }
}
