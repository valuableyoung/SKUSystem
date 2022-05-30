using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Data.SqlClient;

namespace SKUSystem
{
    public class DatabaseManipulator
    {
        public string ScanCode { get; set; }
        public string SetStatus { get; set; }
        
       
        
        public  string SelectTestQuery = @"Select 
                                                  SKU.SKUname, 
                                                  SKU.SKUDescription,
                                                  SKU.Status
                                                  
                                           From 
                                                   [SKU_DB].[dbo].[SKUView] as Sku
                                           Where 
                                                   SKU.ScanCode  = @ScanCode;";

        public string SelectStatusesQuery = @"Select 
                                                       SKUStatuses.id,
                                                       SKUStatuses.status
                                                       
                                             From 
                                                       [SKU_DB].[dbo].[SKUStatuses]
                                             order by  
                                                       SKUStatuses.id asc ;";


        public string UpdateStatusQuery = @"Update SKU
                                                     set 
                                                            status_id = @SetStatus 
                                                     where 
                                                            scancode = @Scancode;";
                                          



        public void GetData()
        {
            Console.WriteLine("Отсканируйте код:");
            ScanCode = Convert.ToString(Console.ReadLine());

            var con = new SqlConnection(Config.connectiontring);
            var cmd  = new SqlCommand(SelectTestQuery);
            cmd.Parameters.Add("@ScanCode", SqlDbType.NVarChar).Value = ScanCode;

            cmd.Connection = con;

            try
            {
                con.Open();
                
                SqlDataReader readerSKU = cmd.ExecuteReader();

                if (readerSKU.HasRows)
                {
                    while (readerSKU.Read())
                    {
                        Console.WriteLine("Подключение к серверу: Успешно");
                        Console.WriteLine("Наименование: {0}\nОписание: {1}\nСостояние: {2}",
                             readerSKU.GetString(0),
                             readerSKU.GetString(1),
                             readerSKU.GetString(2));
                            


                    }
                    readerSKU.Close();

                    cmd = new SqlCommand(SelectStatusesQuery);
                    cmd.Connection = con;
                    SqlDataReader readerStatus  = cmd.ExecuteReader();

                    Console.WriteLine("Выберите статус: ");
                    
                    while (readerStatus.Read())
                    {
                        Console.WriteLine("{0} - {1}",  readerStatus.GetInt32(0), readerStatus.GetString(1));

                        
                    }
                    readerStatus.NextResult();
                    CheckData();
                    
                }
                else
                {
                    Console.WriteLine("Данные не найдены");
                    Console.ReadKey();
                    Console.Clear();
                    
                    GetData();
                    
                
                }

        }

            catch
            {
           
                //Console.WriteLine("Подключение к серверу: Ошибка");
                throw;
            }
            
        }

        public void SetData(string SetStatus)
        {
            var con = new SqlConnection(Config.connectiontring);
            var cmd = new SqlCommand(UpdateStatusQuery);
            cmd.Parameters.Add("@SetStatus", SqlDbType.Int).Value = SetStatus;
            cmd.Parameters.Add("@ScanCode", SqlDbType.NVarChar).Value = ScanCode;
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine("Статус успешно обновлен");
            Console.ReadKey();
            Console.Clear();
            GetData();

        }

        public void CheckData()
        {
            SetStatus = Console.ReadLine();
            
            if ((SetStatus != "1") && (SetStatus != "2") && (SetStatus != "3"))
            {
                Console.WriteLine("Недопустимый ввод");
                //Console.Clear();
                CheckData();
                  
                 
            }
            else
            {
                Console.WriteLine("1 - подтвердить, любая другая цифра - отмена");
                if (Console.ReadLine() == "1")
                {
                    Console.WriteLine("Подтверждено!\nИдет отправка данных на сервер");
                    SetData(SetStatus);
                }
                else if (Console.ReadLine() != "1")
                {
                    Console.Clear();
                    Console.WriteLine("Отмена подтверждена");
                    GetData();
                }
                
            }
            
            
        }

    }
}
