using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sistema.Entidad;
using Sistema.Models;
using Microsoft.EntityFrameworkCore;

namespace Sistema.AppStart
{
    public class ApplicationDbContext : DbContext
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<asistenciaModel>()
             .HasOne(a => a.colaborador)
             .WithMany(c => c.Asistencias)
             .HasForeignKey(a => a.num_empleado);
            // Ignorar SelectListGroup para que EF no lo trate como entidad
            modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.Rendering.SelectListGroup>();
            modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
              modelBuilder.Entity<UsuarioModel>(entity =>
    {
        entity.ToTable("Usuarios");        // Nombre exacto de la tabla
        entity.HasKey(e => e.Id);          // Llave primaria
        entity.Property(e => e.Id)
              .ValueGeneratedNever();      // <- Importante: no genera IDENTITY
    });

        }


        // Aquí registras tus tablas como DbSet
        public DbSet<ColaboradorModel> Colaboradores { get; set; }
        public DbSet<asistenciaModel> Asistencias { get; set; }
        public DbSet<UsuarioModel> Usuarios { get; set; }

    }
}
