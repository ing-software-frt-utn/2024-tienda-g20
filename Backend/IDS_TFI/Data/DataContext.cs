using Microsoft.EntityFrameworkCore;
using IDS_TFI.Dominio;
using IDS_TFI.Data;

namespace IDS_TFI
{
    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions<DataContext> options)
            : base(options)
        {
			if (Database.EnsureCreated())
            {
				DatosIniciales.Cargar(this);
				SaveChanges();
            }
        }
#if DEBUG
		public bool Reset()
		{
			if (Database.EnsureDeleted() && Database.EnsureCreated())
			{
				DatosIniciales.Cargar(this);
				SaveChanges();
				return true;
			}
			return false;
		}
#endif

		public DbSet<Articulo> Articulos { get; set; }
		public DbSet<Categoria> Categorias { get; set; }
		public DbSet<Cliente> Clientes { get; set; }
		public DbSet<Color> Colores { get; set; }
		public DbSet<Comprobante> Comprobantes { get; set; }
		public DbSet<Direccion> Direcciones { get; set; }
		public DbSet<Empleado> Empleados { get; set; }
		public DbSet<LineaDeVenta> LineasDeVenta { get; set; }
		public DbSet<Marca> Marcas { get; set; }
		public DbSet<Pago> Pagos { get; set; }
		public DbSet<Pais> Paises { get; set; }
		public DbSet<Stock> Stock { get; set; }
		public DbSet<Provincia> Provincias { get; set; }
		public DbSet<PuntoDeVenta> PuntosDeVenta { get; set; }
		public DbSet<Talle> Talles { get; set; }
		public DbSet<TipoDeComprobante> TiposDeComprobantes { get; set; }
		public DbSet<TipoDeTalle> TiposDeTalle { get; set; }
		public DbSet<Venta> Ventas { get; set; }
	}
}
