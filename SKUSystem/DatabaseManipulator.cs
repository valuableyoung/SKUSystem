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
        public DateTime Date { get; set; }
        
       
        
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
                                                            status_id = @SetStatus,
                                                            Change_date = @Date 
                                                     where  
                                                            scancode = @Scancode;";
                                          



        public void GetData(string scancode)
        {
            
            ScanCode = scancode;
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
                    NextSession(2);
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
            Date = DateTime.Now;
            cmd.Parameters.Add("@SetStatus", SqlDbType.Int).Value = SetStatus;
            cmd.Parameters.Add("@ScanCode", SqlDbType.NVarChar).Value = ScanCode;
            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = Date;

            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();

            NextSession(0);
            
        }
        
        public void NextSession(int a)
        {
            Console.Clear();
            int variant = a;
            switch (variant)
            {
                case 0: Console.WriteLine("Статус успешно обновлен");
                        Console.WriteLine("Отсканируйте код:");
                    break;
                case 1: Console.WriteLine("Отмена подтверждена");
                        Console.WriteLine("Отсканируйте код:");
                    break;
                case 2: Console.WriteLine("Данные не найдены");
                        Console.WriteLine("Отсканируйте код:");
                    break;
                case 3: Console.WriteLine("Отсканируйте код:");                       
                    break;
                case 4: Console.WriteLine("Отсканируйте код:");
                        GetData(SetStatus);
                    break;
            }
            if (variant != 4)
            {
                string s = Convert.ToString(Console.ReadLine());
                GetData(s);
            }
            
        }

        public void CheckNewScanCode(string SetStatus) 
        {
            if(SetStatus.Contains('@'))
            {
                NextSession(4);
            }
        }

        public void CheckData()
        {
            SetStatus = Console.ReadLine();
            
            if ((SetStatus != "1") && (SetStatus != "2") && (SetStatus != "3"))
            {
                CheckNewScanCode(SetStatus);
                Console.WriteLine("Недопустимый ввод");
                //Console.Clear();
                CheckData();                  
                 
            }
            else

            {
                Console.WriteLine("1 - подтвердить, любая другая цифра - отмена");
                string confirmstatus = Convert.ToString(Console.ReadLine());
                if (confirmstatus == "1")
                { 
               
                    Console.WriteLine("Подтверждено!\nИдет отправка данных на сервер");
                    SetData(SetStatus);
                }
                else if (confirmstatus != "1")
                {
                    CheckNewScanCode(confirmstatus);
                    NextSession(1);
                }
                
            }
            
            
        }

    }
}
