import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';


interface Permiso {
  id: number;
  nombre: string;
  correo: string;
  motivo: string;
  fechaSolicitud: string;
  estado: string;
  archivoPdf?: string;
}

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.scss']
})
export class AdminPanelComponent implements OnInit {
  solicitudes: any[] = [];
  mensaje = '';
  fechaFiltro: string = '';

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.obtenerSolicitudes();
  }

  onFechaChange(): void {
    this.obtenerSolicitudes(this.fechaFiltro);
  }

  pagina: number = 1;
  tamanioPagina: number = 10;
  totalSolicitudes: number = 0;

  obtenerSolicitudes(fecha?: string): void {
    let params = new HttpParams()
      .set('page', this.pagina)
      .set('pageSize', this.tamanioPagina);

    if (fecha) {
      params = params.set('fecha', fecha);
    }

    this.http.get<any>('http://localhost:5206/api/permisos/solicitudes',
      { params }
    )
      .subscribe({
        // next: (data) => this.solicitudes = data,
        next: (data) => {
          this.solicitudes = data.data
          console.log(this.solicitudes);
          this.totalSolicitudes = data.total;
        },
        error: () => this.mensaje = 'Error al cargar las solicitudes.'
      });
  }

  cambiarPagina(nuevaPagina: number): void {
    this.pagina = nuevaPagina;
    this.obtenerSolicitudes(this.fechaFiltro);
  }

  actualizarEstado(id: number, estado: string) {
    const body = { Estado: estado };  // debe coincidir con EstadoPermisoDto

    this.http.put<{ mensaje: string }>(`http://localhost:5206/api/permisos/${id}`, body)
      .subscribe({
        next: (res) => {
          this.mensaje = res.mensaje;
          this.obtenerSolicitudes(this.fechaFiltro); // Refrescar
        },
        error: (err) => {
          console.error('Error actualizando estado:', err);
          this.mensaje = 'Error al actualizar estado.';
        }
      });
  }

  exportarAExcel(): void {
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.solicitudes);
    const workbook: XLSX.WorkBook = { Sheets: { 'Solicitudes': worksheet }, SheetNames: ['Solicitudes'] };
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const nombreArchivo = `Solicitudes_${new Date().toISOString().slice(0, 10)}.xlsx`;
    const blob: Blob = new Blob([excelBuffer], { type: 'application/octet-stream' });
    FileSaver.saveAs(blob, nombreArchivo);
  }

  exportarAPdf(): void {
    const doc = new jsPDF();

    doc.text('Solicitudes de Permiso', 14, 15);

    autoTable(doc, {
      startY: 20,
      head: [['ID', 'Nombre', 'Correo', 'Motivo', 'Fecha', 'Estado']],
      body: this.solicitudes.map(s => [
        s.id,
        s.nombre,
        s.correo,
        s.motivo,
        new Date(s.fechaSolicitud).toLocaleDateString(),
        s.estado
      ])
    });

    doc.save(`Solicitudes_${new Date().toISOString().slice(0,10)}.pdf`);
  }
}