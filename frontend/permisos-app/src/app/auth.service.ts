import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'http://localhost:5206/api/auth';

  constructor(private http: HttpClient) {}

  login(datos: { correo: string; contrasena: string }) {
  return this.http.post<{ token: string, rol: string }>(`${this.apiUrl}/login`, datos);
}

  guardarToken(token: string) {
    localStorage.setItem('token', token);
  }

  obtenerToken(): string | null {
    return localStorage.getItem('token');
  }

  // eliminarToken() {
  //   localStorage.removeItem(this.tokenKey);
  // }

  eliminarToken() {
  localStorage.removeItem('token');
  localStorage.removeItem('rol');
}

  isLoggedIn(): boolean {
  const token = this.obtenerToken();
  if (!token) return false;

  const payload = JSON.parse(atob(token.split('.')[1]));
  const expiracion = payload.exp * 1000; // en milisegundos
  return Date.now() < expiracion;
}

  getRol(): string | null {
    return localStorage.getItem('rol');
  }
}