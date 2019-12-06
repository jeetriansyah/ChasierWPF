namespace CRUDBootcamp32.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModelRegister : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_m_items",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NameItem = c.String(),
                        Stock = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                        Supplier_Id = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.tb_m_supplier", t => t.Supplier_Id)
                .Index(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.tb_m_supplier",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        CreateDate = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tb_m_role",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.tb_t_transactiondetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        SubTotal = c.Int(nullable: false),
                        Items_ID = c.Int(),
                        Transactions_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.tb_m_items", t => t.Items_ID)
                .ForeignKey("dbo.tb_m_transaction", t => t.Transactions_ID)
                .Index(t => t.Items_ID)
                .Index(t => t.Transactions_ID);
            
            CreateTable(
                "dbo.tb_m_transaction",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TransactionDate = c.DateTimeOffset(nullable: false, precision: 7),
                        TotalPrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.tb_m_user",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        RegisterDate = c.DateTimeOffset(nullable: false, precision: 7),
                        Roles_ID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_m_role", t => t.Roles_ID)
                .Index(t => t.Roles_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_m_user", "Roles_ID", "dbo.tb_m_role");
            DropForeignKey("dbo.tb_t_transactiondetail", "Transactions_ID", "dbo.tb_m_transaction");
            DropForeignKey("dbo.tb_t_transactiondetail", "Items_ID", "dbo.tb_m_items");
            DropForeignKey("dbo.tb_m_items", "Supplier_Id", "dbo.tb_m_supplier");
            DropIndex("dbo.tb_m_user", new[] { "Roles_ID" });
            DropIndex("dbo.tb_t_transactiondetail", new[] { "Transactions_ID" });
            DropIndex("dbo.tb_t_transactiondetail", new[] { "Items_ID" });
            DropIndex("dbo.tb_m_items", new[] { "Supplier_Id" });
            DropTable("dbo.tb_m_user");
            DropTable("dbo.tb_m_transaction");
            DropTable("dbo.tb_t_transactiondetail");
            DropTable("dbo.tb_m_role");
            DropTable("dbo.tb_m_supplier");
            DropTable("dbo.tb_m_items");
        }
    }
}
