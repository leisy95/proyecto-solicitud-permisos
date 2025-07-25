import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-crear-usuario',
  templateUrl: './crear-usuario.component.html'
})
export class CrearUsuarioComponent {
  usuarioForm: FormGroup;
  mensaje = '';
  error = '';

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.usuarioForm = this.fb.group({
      nombre: ['', Validators.required],
      correo: ['', [Validators.required, Validators.email]],
      rol: ['Usuario', Validators.required]
    });
  }

  crearUsuario(): void {
    this.mensaje = '';
    this.error = '';

    if (this.usuarioForm.invalid) {
      this.error = 'Por favor completa todos los campos correctamente.';
      return;
    }

    const datos = this.usuarioForm.value;

    this.http.post('http://localhost:5206/api/usuarios/crear-usuario', datos).subscribe({
      next: () => {
        this.mensaje = 'Usuario creado y correo enviado correctamente.';
        this.usuarioForm.reset({ rol: 'Usuario' });
      },
      error: (err) => {
        console.error(err);
        this.error = err.error?.mensaje || 'Error al crear el usuario.';
      }
    });
  }
}