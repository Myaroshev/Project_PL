using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewProject_PL
{
    class Report
    {
        private string _lastName;
        private string _firstName;
        private string _isbn;
        private DateTime _issuanceDate;
        private DateTime _returnDate;
        private string _expiredStatus;

        //фамилия
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        //имя
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        //ISBN книги
        public string ISBN
        {
            get { return _isbn; }
            set { _isbn = value; }
        }
        //дата выдачи
        public DateTime IssuanceDate
        {
            get { return _issuanceDate; }
            set { _issuanceDate = value; }
        }
        //дача возврата
        public DateTime ReturnDate
        {
            get { return _returnDate; }
            set { _returnDate = value; }
        }
        //статус просрочки
        public string ExpiredStatus
        {
            get { return _expiredStatus; }
            set { _expiredStatus = value; }
        }
    }
}
