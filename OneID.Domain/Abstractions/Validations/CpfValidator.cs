using OneID.Domain.Notifications;

namespace OneID.Domain.Abstractions.Validations
{
    public partial class ContractValidations<T>
    {
        public ContractValidations<T> CpfIsOK(string cpf, bool foreignWorker, string message, string propertyName)
        {
            if (!IsCpf(cpf, foreignWorker))
                AddNotification(new Notification(message, propertyName));

            return this;
        }

        internal bool IsCpf(string cpf, bool foreignWorker)
        {
            if (foreignWorker)
            {
                return true;
            }
            if (string.IsNullOrEmpty(cpf))
            {
                return false;
            }

            int[] multiplier1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multiplier2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            if (cpf.Length != 11) return false;

            for (int j = 0; j < 10; j++)
                if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == cpf)
                    return false;

            string tempCpf = cpf[..9];
            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

            int mod = sum % 11;
            if (mod < 2)
                mod = 0;
            else
                mod = 11 - mod;

            string digit = mod.ToString();
            tempCpf += digit;
            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

            mod = sum % 11;
            if (mod < 2)
                mod = 0;
            else
                mod = 11 - mod;

            digit += mod.ToString();

            return cpf.EndsWith(digit);
        }

    }

}
