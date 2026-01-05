using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unı_Proje.Models;

namespace Unı_Proje.Data
{
    public class ProjeDbContext : DbContext
    {
        public ProjeDbContext(DbContextOptions<ProjeDbContext> options)
            : base(options)
        {
        }

        // DbContext'e tabloları tanıtıyoruz
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Urun> Urunler { get; set; }

        public DbSet<SepetUrun> SepetUrunleri { get; set; }
        public DbSet<Favori> Favoriler { get; set; }
        public DbSet<Mesaj> Mesajlar { get; set; }
        public DbSet<SifreResetToken> SifreResetTokenleri { get; set; }
        
        // Email Doğrulama Tokenları
        public DbSet<EmailDogrulamaToken> EmailDogrulamaTokenleri { get; set; }
        
        // Sipariş tabloları
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylari { get; set; }
        
        // Satıcı Değerlendirme
        public DbSet<SaticiDegerlendirme> SaticiDegerlendirmeleri { get; set; }
        
        // Bildirimler
        public DbSet<Bildirim> Bildirimler { get; set; }
        
        // Teklifler (Pazarlık)
        public DbSet<Teklif> Teklifler { get; set; }
        
        // ChatBot Mesajları
        public DbSet<ChatMesaj> ChatMesajlari { get; set; }
        
        // 📸 Çoklu Resimler
        public DbSet<UrunResim> UrunResimleri { get; set; }

        // Tablo isimlerini MySQL uyumlu küçük harfe çevirme ayarı (MySQL'de önemlidir)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kullanici>().ToTable("kullanicilar");
            modelBuilder.Entity<Kategori>().ToTable("kategoriler");
            modelBuilder.Entity<Urun>().ToTable("urunler");
            modelBuilder.Entity<SepetUrun>().ToTable("sepet_urunleri");
            modelBuilder.Entity<Favori>().ToTable("favoriler");
            modelBuilder.Entity<Mesaj>().ToTable("mesajlar");
            modelBuilder.Entity<Siparis>().ToTable("siparisler");
            modelBuilder.Entity<SiparisDetay>().ToTable("siparis_detaylari");

            // Mesaj tablosu için özel ayarlar
            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Gonderen)
                .WithMany()
                .HasForeignKey(m => m.GonderenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Alici)
                .WithMany()
                .HasForeignKey(m => m.AliciId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Urun)
                .WithMany()
                .HasForeignKey(m => m.UrunId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SifreResetToken>().ToTable("sifre_reset_tokenleri");
            
            // Email Doğrulama Tokenları tablosu
            modelBuilder.Entity<EmailDogrulamaToken>().ToTable("email_dogrulama_tokenleri");

            // Satıcı Değerlendirme tablosu
            modelBuilder.Entity<SaticiDegerlendirme>().ToTable("satici_degerlendirmeleri");
            
            // Satıcı-Değerlendiren ilişkileri
            modelBuilder.Entity<SaticiDegerlendirme>()
                .HasOne(d => d.Satici)
                .WithMany()
                .HasForeignKey(d => d.SaticiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaticiDegerlendirme>()
                .HasOne(d => d.Degerlendiren)
                .WithMany()
                .HasForeignKey(d => d.DegerlendirenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bildirimler tablosu
            modelBuilder.Entity<Bildirim>().ToTable("bildirimler");

            // Teklifler tablosu
            modelBuilder.Entity<Teklif>().ToTable("teklifler");
            
            // Teklif-Ürün ilişkisi
            modelBuilder.Entity<Teklif>()
                .HasOne(t => t.Urun)
                .WithMany()
                .HasForeignKey(t => t.UrunId)
                .OnDelete(DeleteBehavior.Cascade);

            // Teklif-Alıcı ilişkisi
            modelBuilder.Entity<Teklif>()
                .HasOne(t => t.Alici)
                .WithMany()
                .HasForeignKey(t => t.AliciId)
                .OnDelete(DeleteBehavior.Restrict);

            // ChatBot Mesajları tablosu
            modelBuilder.Entity<ChatMesaj>().ToTable("chat_mesajlari");
            
            // 📸 Ürün Resimleri tablosu
            modelBuilder.Entity<UrunResim>().ToTable("urun_resimleri");
            
            // Ürün-Resim ilişkisi
            modelBuilder.Entity<UrunResim>()
                .HasOne(r => r.Urun)
                .WithMany(u => u.Resimler)
                .HasForeignKey(r => r.UrunId)
                .OnDelete(DeleteBehavior.Cascade); // Ürün silinince resimleri de silinsin
        }
    }
}