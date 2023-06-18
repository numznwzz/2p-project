using Domain.MongoDB.Collections._2p;
using Mongo.Migration.Migrations.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.MongoDB.Migration
{
    public class InitNew1 : DatabaseMigration
    {
        public InitNew1(string version) : base("0.0.1")
        {
        }

        public override void Up(IMongoDatabase db)
        {
            
            //menu
            var collectionMenu = db.GetCollection<Menu>("menu");
            
            collectionMenu.InsertOne(new Menu()
            {
                Name = "รายงานภาพรวม"
            });

            List<SubMenu> subMenus = new List<SubMenu>();
            subMenus.Add(new SubMenu()
            {
              Id  = ObjectId.GenerateNewId(),
              Name = "ลพบุรี"
            });
            
            subMenus.Add(new SubMenu()
            {
                Id  = ObjectId.GenerateNewId(),
                Name = "สระบุรี"
            });
            
            collectionMenu.InsertOne(new Menu()
            {
                Name = "รายงานแต่ละสาขา",
                SubMenu = subMenus
            });
            
            
            collectionMenu.InsertOne(new Menu()
            {
                Name = "รายชื่อพนักงานแต่ละสาขา",
                SubMenu = subMenus
            });
            
            collectionMenu.InsertOne(new Menu()
            {
                Name = "เครื่องจักรแต่ละสาขา",
                SubMenu = subMenus
            });
            
            collectionMenu.InsertOne(new Menu()
            {
                Name = "รายชื่อพนักงานแต่ละสาขา",
                SubMenu = subMenus
            });
            
            collectionMenu.InsertOne(new Menu()
            {
                Name = "บันทึกยอดการผลิตแต่ละสาขา",
                SubMenu = subMenus
            });

            var collectionFactory = db.GetCollection<Factory>("factory");
            
            collectionFactory.InsertOne(new Factory()
            {
                Name = "สระบุรี",
                Address = "99 หมู่ 9 ตำบลหัวปลวก อำเภอเสาไห้ จังหวัดสระบุรี 18160",
                Tel = "0927779887",
                Slug = "FACTORY_SARABURI"
            });
            
            collectionFactory.InsertOne(new Factory()
            {
                Name = "ลพบุรี",
                Address = "188/5 - 188/6 หมู่5 ตำบลทะเลชุบศร อำเภอเมือง จังหวัดลพบุรี 15000",
                Tel = "0958976468",
                Slug = "FACTORY_LOPBURI"
            });
            
            var collectionEmployee = db.GetCollection<Employee>("employee");
            
            collectionEmployee.InsertOne(new Employee()
            {
                Fname = "n",
                Lname = "rockman",
            });

            var em = db.GetCollection<Employee>("employee").AsQueryable().Where(x=>x.Fname=="n").FirstOrDefault();

            if (em != null)
            {
                var collectionAuth = db.GetCollection<Login>("login");
                collectionAuth.InsertOne(new Login
                {
                    Username = "numznwzz",
                    Password = "123321",
                    EmployeeId = em.Id,
                    RoleId = new ObjectId()
                });
            }
            
        }

        public override void Down(IMongoDatabase db)
        {
        }
    }
}