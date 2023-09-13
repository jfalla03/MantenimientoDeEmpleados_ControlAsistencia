using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication1.Models
{
    public partial class EFFICALContext : DbContext
    {
        public EFFICALContext()
        {
        }

        public EFFICALContext(DbContextOptions<EFFICALContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ControlDeAsistencium> ControlDeAsistencia { get; set; }
        public virtual DbSet<Empleado> Empleados { get; set; }
        public virtual DbSet<RecuperarContraseña> RecuperarContraseñas { get; set; }
        public virtual DbSet<SolicitudesJustificacion> SolicitudesJustificacions { get; set; }
        public virtual DbSet<SueldoEmpleado> SueldoEmpleados { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=LAPTOP-RMLV3UJ2;database=EFFICAL;integrated security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ControlDeAsistencium>(entity =>
            {
                entity.HasKey(e => e.CodAsistencia)
                    .HasName("PK__CONTROL___0A6F21E15305465A");

                entity.ToTable("CONTROL_DE_ASISTENCIA");

                entity.Property(e => e.CodAsistencia)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("COD_ASISTENCIA")
                    .IsFixedLength();

                entity.Property(e => e.CodEmpleado)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("COD_EMPLEADO")
                    .IsFixedLength();

                entity.Property(e => e.EstadoAsistencia)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ESTADO_ASISTENCIA");

                entity.Property(e => e.FechAsistencia)
                    .HasColumnType("date")
                    .HasColumnName("FECH_ASISTENCIA");

                entity.Property(e => e.HoraDescansoFin).HasColumnName("HORA_DESCANSO_FIN");

                entity.Property(e => e.HoraDescansoInicio).HasColumnName("HORA_DESCANSO_INICIO");

                entity.Property(e => e.HoraEntrada).HasColumnName("HORA_ENTRADA");

                entity.Property(e => e.HoraSalida).HasColumnName("HORA_SALIDA");

                entity.HasOne(d => d.CodEmpleadoNavigation)
                    .WithMany(p => p.ControlDeAsistencia)
                    .HasForeignKey(d => d.CodEmpleado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CONTROL_D__COD_E__2F10007B");
            });

            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.HasKey(e => e.CodEmpleado)
                    .HasName("PK__EMPLEADO__26C2D72819B86AF2");

                entity.ToTable("EMPLEADO");

                entity.Property(e => e.CodEmpleado)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("COD_EMPLEADO")
                    .IsFixedLength();

                entity.Property(e => e.ApeEmpleado)
                    .IsRequired()
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("APE_EMPLEADO");

                entity.Property(e => e.DirecMpleado)
                    .IsRequired()
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("DIREC_MPLEADO");

                entity.Property(e => e.DniEmpleado)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DNI_EMPLEADO")
                    .IsFixedLength();

                entity.Property(e => e.EmailEmpleado)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL_EMPLEADO");

                entity.Property(e => e.NomEmpleado)
                    .IsRequired()
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("NOM_EMPLEADO");

                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.TeleEmpleado)
                    .IsRequired()
                    .HasMaxLength(9)
                    .IsUnicode(false)
                    .HasColumnName("TELE_EMPLEADO")
                    .IsFixedLength();

                entity.HasOne(d => d.NombreUsuarioNavigation)
                    .WithMany(p => p.Empleados)
                    .HasForeignKey(d => d.NombreUsuario)
                    .HasConstraintName("FK__EMPLEADO__Nombre__29572725");
            });

            modelBuilder.Entity<RecuperarContraseña>(entity =>
            {
                entity.HasKey(e => e.Nombreusuario)
                    .HasName("PK__Recupera__411D9B21C8B5C017");

                entity.Property(e => e.Nombreusuario)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NOMBREUSUARIO")
                    .IsFixedLength();

                entity.Property(e => e.Expiracion).HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.NombreusuarioNavigation)
                    .WithOne(p => p.RecuperarContraseña)
                    .HasForeignKey<RecuperarContraseña>(d => d.Nombreusuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Recuperar__Expir__267ABA7A");
            });

            modelBuilder.Entity<SolicitudesJustificacion>(entity =>
            {
                entity.HasKey(e => e.CodSolicitud)
                    .HasName("PK__Solicitu__307A4CAFBB1249B8");

                entity.ToTable("SolicitudesJustificacion");

                entity.Property(e => e.CodEmpleado)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.FechaSolicitud).HasColumnType("datetime");

                entity.Property(e => e.Motivo)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.CodEmpleadoNavigation)
                    .WithMany(p => p.SolicitudesJustificacions)
                    .HasForeignKey(d => d.CodEmpleado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Solicitud__CodEm__3E52440B");
            });

            modelBuilder.Entity<SueldoEmpleado>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CodEmpleado)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Comentarios).HasMaxLength(255);

                entity.Property(e => e.FechaPago)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Impuestos).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Periodo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.CodEmpleadoNavigation)
                    .WithMany(p => p.SueldoEmpleados)
                    .HasForeignKey(d => d.CodEmpleado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SueldoEmp__CodEm__412EB0B6");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Nombre)
                    .HasName("PK__USUARIO__75E3EFCEF60E12D6");

                entity.ToTable("USUARIO");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Clave)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Roles)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
