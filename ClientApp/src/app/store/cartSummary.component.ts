import { Component } from '@angular/core';
import { Cart } from '../models/cart.model';

@Component({
  selector: 'app-store-cartsummary',
  templateUrl: 'cartSummary.component.html'
})
export class CartSummaryComponent {
  constructor(private cart: Cart) { }

  get itemCount() {
    return this.cart.itemCount;
  }

  get totalPrice() {
    return this.cart.totalPrice;
  }
}
