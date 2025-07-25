import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-solicitud-permiso',
  templateUrl: './solicitud-permiso.component.html',
  styleUrls: ['./solicitud-permiso.component.scss']
})
export class SolicitudPermisoComponent {
  permiso = {
    nombre: '',
    correo: '',
    motivo: '',
    archivo: null as File | null
  };

  mensaje = '';
  errores: any = {}; //Guardar errores por campo
  archivoSeleccionado?: File;

  constructor(private http: HttpClient) { }

  onArchivoSeleccionado(event: any): void {
    const archivo = event.target.files[0];
    if (archivo) {
      this.permiso.archivo = archivo;
    }
  }

  enviarSolicitud(): void {
    this.mensaje = '';
    this.errores = {}; // Limpia errores anteriores

    const formData = new FormData();
    formData.append('nombre', this.permiso.nombre);
    formData.append('correo', this.permiso.correo);
    formData.append('motivo', this.permiso.motivo);

    if (this.permiso.archivo) {
      formData.append('archivo', this.permiso.archivo);
    }

    this.http.post<{ mensaje: string }>(
      'http://localhost:5206/api/permisos',
      formData,
      {
        headers: {} 
      }
    ).subscribe({
      next: (respuesta) => {
        this.mensaje = respuesta.mensaje;
        this.errores = {};
        this.permiso = { nombre: '', correo: '', motivo: '', archivo: null };
        this.archivoSeleccionado = undefined;
      },
      error: (err) => {
        if (err.status === 400 && err.error && err.error.errors) {
          this.errores = err.error.errors;
          this.mensaje = '';
        } else {
          this.mensaje = 'Ocurrió un error inesperado al enviar la solicitud.';
        }
      }
    });
  }
}