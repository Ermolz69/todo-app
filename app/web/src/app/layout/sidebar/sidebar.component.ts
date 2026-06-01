import { Component, ViewChild } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CategoryListComponent } from '../../features/categories/category-list/category-list.component';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CategoryListComponent],
  templateUrl: './sidebar.component.html'
})
export class SidebarComponent {
  @ViewChild('categoryList') categoryList!: CategoryListComponent;

  openCategoryForm() {
    this.categoryList.openCreateForm();
  }
}
