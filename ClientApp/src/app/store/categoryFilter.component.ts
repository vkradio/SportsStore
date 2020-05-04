import { Component } from '@angular/core';
import { NavigationService } from '../models/navigation.service';

@Component({
  selector: 'app-store-categoryfilter',
  templateUrl: 'categoryFilter.component.html'
})
export class CategoryFilterComponent {
  constructor(public service: NavigationService) { }
}
