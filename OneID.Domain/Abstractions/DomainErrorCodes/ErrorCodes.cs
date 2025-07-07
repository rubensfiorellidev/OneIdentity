using System.Collections.Frozen;

namespace OneID.Domain.Abstractions.DomainErrorCodes
{
    public static class ErrorCodes
    {
        public enum ErrorCategory
        {
            Auth,
            Database,
            ActiveDiretory,
            Validation,
            Service,
            Domain
        }

        public const string AuthInvalidCredentials = "AUTH-INVALID_CREDENTIALS-1001";
        public const string AuthUnauthorizedAccess = "AUTH-UNAUTHORIZED_ACCESS-1002";

        public const string DbConnectionFailed = "DB-CONNECTION_FAILED-2001";
        public const string DbRecordNotFound = "DB-RECORD_NOT_FOUND-2002";
        public const string DbRecordUpdateFailed = "DB-RECORD_UPDATE-3001";
        public const string DbConcurrencyError = "DB-CONCURRENCY_ERROR-3002";
        public const string DbRetrievalFailure = "DB-RETRIEVAL_FAILURE-3003";
        public const string DbException = "DB-INSERT_OR_UPDATE_FAILURE-3004";
        public const string DbSqlState = "DB-INSERT_NEW_KEY_FAILURE-3005";


        public const string ValidationUniqueConstraintViolation = "VALIDATION-UNIQUE-CONSTRAINT-4000";
        public const string ValidationCpfAlreadyExists = "VALIDATION-CPF_ALREADY_EXISTS-4001";
        public const string ValidationSlugAlreadyExists = "VALIDATION-SLUG_ALREADY_EXISTS-4003";
        public const string ValidationLoginAlreadyExists = "VALIDATION-LOGIN_ALREADY_EXISTS-4004";
        public const string ValidationEmailAlreadyExists = "VALIDATION-EMAIL_ALREADY_EXISTS-4005";
        public const string ValidationNifAlreadyExists = "VALIDATION-NIF_ALREADY_EXISTS-4006";
        public const string ValidationNameAlreadyExists = "VALIDATION-NAME_ALREADY_EXISTS-4007";

        public const string ValidationInvalidInput = "VALIDATION-INVALID_INPUT-4002";
        public const string ValidationMissingField = "VALIDATION-MISSING_FIELD-5100";
        public const string ValidationNifRequired = "VALIDATION-NIF_REQUIRED-5101";
        public const string ValidationFailureDateClosing = "DATE_CLOSING_INAVLID-5102";

        public const string ServiceUnavailable = "SRV-SERVICE_UNAVAILABLE-5000";
        public const string ServiceTimeout = "SRV-TIMEOUT-5001";
        public const string ServiceUnexpectedError = "SRV-UNEXPECTED_ERROR-5002";

        public const string AdConnectionFailed = "AD-CONNECTION_FAILED-7001";
        public const string AdRecordNotFound = "AD-RECORD_NOT_FOUND-7002";
        public const string AdRecordUpdateFailed = "AD-RECORD_UPDATE-7003";
        public const string AdConcurrencyError = "AD-CONCURRENCY_ERROR-7004";
        public const string AdRetrievalFailure = "AD-RETRIEVAL-FAILURE-7005";
        public const string AdException = "AD-INSERT-OR-UPDATE-FAILURE-7006";
        public const string AdConflict = "AD-UNIQUE-CONSTRAINT-7007";

        public const string DomainEndDateInvalid = "END_DATE_INVALID-4099";
        public const string DomainEntityNotFound = "ENTITY_NOT_FOUND-4100";

        public const string FormatException = "STRING_NOT_BASE_64-8001";
        public const string ServiceCryptoError = "SRV-CRYPTO_ERROR-5003";


        private static readonly Dictionary<string, string> ConstraintFieldMessages = new()
        {
            { ValidationCpfAlreadyExists, "CPF" },
            { ValidationLoginAlreadyExists, "Login" },
            { ValidationEmailAlreadyExists, "Email" },
            { ValidationNifAlreadyExists, "NIF" },
            { ValidationNameAlreadyExists, "Name" }

        };

        public static readonly FrozenDictionary<string, string> Errors = new Dictionary<string, string>
        {
            { AuthInvalidCredentials, "Invalid credentials provided." },
            { AuthUnauthorizedAccess, "Unauthorized access attempt detected." },

            { DbConnectionFailed, "Database connection failed." },
            { DbRecordNotFound, "Record not found in the database." },
            { DbRecordUpdateFailed, "Failed to update the record, Rollback applied." },
            { DbConcurrencyError, "A concurrency error occurred during the database update." },
            { DbRetrievalFailure, "An error occurred while retrieving data." },
            { DbException, "Violates Foreign Key Constraint." },
            { DbSqlState, "Violation of uniqueness constraint in constraint"  },

            { AdConnectionFailed, "Active Directory connection failed." },
            { AdRecordNotFound, "User not found in the AD." },
            { AdRecordUpdateFailed, "Failed to update the user." },
            { AdConcurrencyError, "A concurrency error occurred during the AD update." },
            { AdRetrievalFailure, "An error occurred while retrieving user." },
            { AdException, "Violates Constraint." },

            { ValidationInvalidInput, "The input provided is invalid." },
            { ValidationMissingField, "A required field is missing." },
            { ValidationNifRequired, "Enter the NIF of the foreign service provider." },
            { ValidationFailureDateClosing, "Closing date, invalid" },

            { ServiceUnavailable, "The service is currently unavailable." },
            { ServiceTimeout, "The operation timed out." },
            { ServiceUnexpectedError, "An unexpected error occurred." },

            { DomainEndDateInvalid, "The end date cannot be earlier than today's date." },
            { DomainEntityNotFound, "Service provider not found in the system." },

            { FormatException, "The string provided is not valid Base-64." },
            { ServiceCryptoError, "An error occurred during cryptographic operations." }


        }.ToFrozenDictionary();

        public static string GetErrorMessage(string code)
        {
            return Errors.TryGetValue(code, out string? value) ? value : "Unknown error code.";
        }

        public static string GetErrorMessageByCategory(string code, ErrorCategory category)
        {
            return category switch
            {
                ErrorCategory.Auth => "Authentication/Authorization Error: " + GetErrorMessage(code),
                ErrorCategory.Database => "Database Error: " + GetErrorMessage(code),
                ErrorCategory.ActiveDiretory => "Active Directory Error: " + GetErrorMessage(code),
                ErrorCategory.Validation => "Validation Error: " + GetErrorMessage(code),
                ErrorCategory.Service => "Service Error: " + GetErrorMessage(code),
                ErrorCategory.Domain => "Domain Error: " + GetErrorMessage(code),
                _ => "Unknown Error Category."
            };
        }

        public static string GetUniqueConstraintViolationMessage(string code)
        {
            if (ConstraintFieldMessages.TryGetValue(code, out string? field))
            {
                return $"{field} already exists in the system.";
            }

            return "Violation of uniqueness constraint.";
        }
    }

}
