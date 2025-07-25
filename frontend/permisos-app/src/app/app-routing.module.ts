import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminPanelComponent } from './admin-panel/admin-panel.component';
import { SolicitudPermisoComponent } from './solicitud-permiso/solicitud-permiso.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './auth.guard';
import { CrearUsuarioComponent } from './admin/crear-usuario/crear-usuario.component';

const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'solicitud', component: SolicitudPermisoComponent },
  { path: 'admin', component: AdminPanelComponent, canActivate: [AuthGuard] },
  {
    path: 'admin/crear-usuario',
    component: CrearUsuarioComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
