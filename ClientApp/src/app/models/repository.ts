import { Product } from './product.model';
import { Supplier } from './supplier.model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Filter, Pagination } from './configClasses.repository';
import { Observable } from 'rxjs';

const productsUrl = '/api/products';
const suppliersUrl = '/api/suppliers';
const sessionUrl = '/api/session';

interface ProductsMetadata {
  data: Product[];
  categories: string[];
}

@Injectable()
export class Repository {
  product: Product;
  products: Product[];
  suppliers: Supplier[] = [];
  filter = new Filter();
  categories: string[] = [];
  paginationObject = new Pagination();

  constructor(private http: HttpClient) {
    this.filter.related = true;
  }

  getProduct(id: number) {
    this
      .http
      .get<Product>(`${productsUrl}/${id}`)
      .subscribe(p => this.product = p);
    console.log('request');
  }

  getProducts(related = false) {
    let url = `${productsUrl}?related=${this.filter.related}`;
    if (this.filter.category) {
      url += `&category=${this.filter.category}`;
    }
    if (this.filter.search) {
      url += `&search=${this.filter.search}`;
    }
    url += '&metadata=true';

    this
      .http
      .get<ProductsMetadata>(url)
      .subscribe(md => {
        this.products = md.data;
        this.categories = md.categories;
      });
  }

  getSuppliers() {
    this
      .http
      .get<Supplier[]>(suppliersUrl)
      .subscribe(sups => this.suppliers = sups);
  }

  createProduct(prod: Product) {
    const data = {
      name: prod.name,
      category: prod.category,
      description: prod.description,
      price: prod.price,
      supplier: prod.supplier ? prod.supplier.supplierId : 0
    };

    this
      .http
      .post<number>(productsUrl, data)
      .subscribe(id => {
        prod.productId = id;
        this.products.push(prod);
      });
  }

  createProductAndSupplier(prod: Product, supp: Supplier) {
    const data = {
      name: supp.name,
      city: supp.city,
      state: supp.state
    };

    this
      .http
      .post<number>(suppliersUrl, data)
      .subscribe(id => {
        supp.supplierId = id;
        prod.supplier = supp;
        this.suppliers.push(supp);
        if (prod != null) {
          this.createProduct(prod);
        }
      });
  }

  replaceProduct(prod: Product) {
    const data = {
      name: prod.name,
      category: prod.category,
      description: prod.description,
      price: prod.price,
      supplier: prod.supplier ? prod.supplier.supplierId : 0
    };

    this
      .http
      .put(`${productsUrl}/${[prod.productId]}`, data)
      .subscribe(() => this.getProducts());
  }

  replaceSupplier(supp: Supplier) {
    const data = {
      name: supp.name,
      city: supp.city,
      state: supp.state
    };

    this
      .http
      .put(`${suppliersUrl}/${supp.supplierId}`, data)
      .subscribe(() => this.getProducts());
  }

  updateProduct(id: number, changes: Map<string, any>) {
    const patch = [];

    changes
      .forEach((val, key) => patch.push({ op: 'replace', path: key, value: val }));

    this
      .http
      .patch(`${productsUrl}/${id}`, patch)
      .subscribe(() => this.getProducts());
  }

  deleteProduct(id: number) {
    this
      .http
      .delete(`${productsUrl}/${id}`)
      .subscribe(() => this.getProducts());
  }

  deleteSupplier(id: number) {
    this
      .http
      .delete(`${suppliersUrl}/${id}`)
      .subscribe(() => {
        this.getProducts();
        this.getSuppliers();
      });
  }

  storeSessionData<T>(dataType: string, data: T) {
    return this
      .http
      .post(`${sessionUrl}/${dataType}`, data)
      .subscribe(response => { });
  }

  getSessionData<T>(dataType: string): Observable<T> {
    return this
      .http
      .get<T>(`${sessionUrl}/${dataType}`);
  }
}
