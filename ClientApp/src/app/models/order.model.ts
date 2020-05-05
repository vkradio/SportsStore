import { Injectable } from '@angular/core';
import { Cart } from './cart.model';
import { Repository } from './repository';

@Injectable()
export class Order {
  orderId: number;
  name?: string;
  address?: string;
  payment = new Payment();
  submitted = false;
  shipped = false;
  orderConfirmation: OrderConfirmation;

  constructor(private repo: Repository, public cart: Cart) { }

  get products() {
    return this
      .cart
      .selections
      .map(p => new CartLine(p.productId, p.quantity));
  }

  clear() {
    this.name = null;
    this.address = null;
    this.payment = new Payment();
    this.cart.clear();
    this.submitted = false;
  }

  submit() {
    this.submitted = true;
    this.repo.createOrder(this);
  }
}

export class Payment {
  cardNumber: string;
  cardExpiry: string;
  cardSecurityCode: string;
}

export class CartLine {
  constructor(private productId: number, private quantity: number) { }
}

export class OrderConfirmation {
  constructor(
    public orderId: number,
    public authCode: string,
    public amount: number) { }
}
