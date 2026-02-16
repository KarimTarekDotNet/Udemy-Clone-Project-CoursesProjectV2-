# 🎓 UCourses - Online Learning Platform Backend API

A comprehensive RESTful API for an online learning management system built with ASP.NET Core 10.0, featuring course management, payment integration, real-time chat, and role-based access control.

---

## 🌟 Features

### 👥 User Management
- Multi-role authentication (Admin, Instructor, Student)
- JWT token-based authentication with refresh tokens
- Email & phone verification (SendGrid + Twilio)
- Google OAuth 2.0 integration
- Password reset flow
- Profile management with image upload

### 📚 Course Management
- Create, update, and publish courses
- Section/lesson organization
- Department categorization
- Course progress tracking
- Video content management
- Student enrollment system

### 💳 Payment Integration
- Paymob payment gateway integration
- Support for cards and mobile wallets
- Secure HMAC callback verification
- Automatic enrollment on successful payment
- Payment status tracking

### 💬 Real-Time Features
- SignalR-based real-time notifications
- Chat system with conversations
- Unread message tracking
- WebSocket support

### 🛡️ Security & Performance
- HTTPS enforcement with HSTS
- Rate limiting (authentication, API, public endpoints)
- Redis distributed caching
- Token blacklisting
- ClamAV virus scanning for uploads
- CORS configuration
- Global exception handling

### 📊 Dashboards
- **Admin Dashboard**: System statistics, user management, course approval
- **Instructor Dashboard**: Course analytics, earnings, student management
- **Student Dashboard**: Enrolled courses, progress tracking, course browsing

### ⚙️ Background Jobs
- Hangfire for scheduled tasks
- Automatic refresh token cleanup
- Recurring job management

---

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 10.0
- **Database**: SQL Server with Entity Framework Core
- **Cache**: Redis
- **Authentication**: JWT Bearer + Google OAuth 2.0
- **Real-time**: SignalR
- **Background Jobs**: Hangfire
- **Email**: SendGrid SMTP
- **SMS**: Twilio Verify API
- **Payment**: Paymob (Egyptian Payment Gateway)
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI
- **Security**: ClamAV Antivirus

---

## 🚀 Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- SQL Server (LocalDB or full instance)
- Redis Server
- ClamAV (optional, for virus scanning)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/ucourses-backend.git
cd ucourses-backend
```

2. **Setup User Secrets**
```bash
cd UCourses_Back_End.Api
dotnet user-secrets init

# Follow the complete guide in UCourses_Back_End.Api/SETUP_SECRETS.md
```

3. **Update Database Connection**
   - Ensure SQL Server is running
   - Connection string is in `appsettings.json`

4. **Run Database Migrations**
```bash
dotnet ef database update
```

5. **Start Redis Server**
```bash
# Windows (if installed)
redis-server

# Or using Docker
docker run -d -p 6379:6379 redis
```

6. **Run the Application**
```bash
dotnet run --urls "https://localhost:7178"
```

7. **Access Swagger Documentation**
```
https://localhost:7178/swagger
```

---

## 📡 API Endpoints

### Base URLs
- **Local**: `https://localhost:7178`
- **Public** (via Ngrok): `https://unmultipliable-kelsey-unloyal.ngrok-free.dev`

### Main Endpoint Categories

- `/api/Auth` - Authentication & registration
- `/api/UserProfile` - User profile management
- `/api/Payment` - Payment processing (Paymob)
- `/api/Chat` - Real-time messaging
- `/api/courses` - Public course browsing
- `/api/departments` - Department management
- `/api/admin` - Admin dashboard (Admin only)
- `/api/instructor` - Instructor dashboard (Instructor only)
- `/api/student` - Student dashboard (Student only)

For complete API documentation, see [BACKEND_FEATURES_PROMPT.md](BACKEND_FEATURES_PROMPT.md)

---

## 🔐 Configuration

### Required Secrets

All sensitive configuration values must be set using User Secrets or Environment Variables:

- JWT Key
- SendGrid Email & API Key
- Twilio Account SID, Auth Token, Phone Number, Verify Service SID
- Google OAuth Client ID & Secret
- Paymob API Key, Secret Key, Public Key, HMAC, Integration ID

**See detailed setup guide**: [UCourses_Back_End.Api/SETUP_SECRETS.md](UCourses_Back_End.Api/SETUP_SECRETS.md)

---

## 🏗️ Project Structure

```
UCourses_Back_End/
├── UCourses_Back_End.Api/          # API Layer
│   ├── Controllers/                # API Controllers
│   ├── Extensions/                 # Service extensions
│   ├── Filters/                    # Action filters
│   ├── Mappings/                   # AutoMapper profiles
│   ├── Middlewares/                # Custom middlewares
│   └── Program.cs                  # Application entry point
├── UCourses_Back_End.Core/         # Domain Layer
│   ├── Entities/                   # Domain entities
│   ├── Interfaces/                 # Repository & service interfaces
│   ├── DTOs/                       # Data transfer objects
│   └── Validators/                 # FluentValidation rules
└── UCourses_Back_End.Infrastructure/ # Infrastructure Layer
    ├── Data/                       # DbContext & configurations
    ├── Repositories/               # Repository implementations
    ├── Services/                   # Service implementations
    └── BackgroundJobs/             # Hangfire jobs
```

---

## 🔧 Development

### Run with Ngrok (for external access)
```bash
# Terminal 1: Start the API
dotnet run --urls "https://localhost:7178"

# Terminal 2: Start Ngrok tunnel
ngrok http https://localhost:7178
```

### Access Hangfire Dashboard
```
https://localhost:7178/hangfire
```
(Admin authentication required)

### Run Tests
```bash
dotnet test
```

---

## 📊 Database

### Default Connection String
```
Server=localhost;Database=UCourse;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;
```

### Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName
```

---

## 🌐 Frontend Integration

The API is designed to work with a frontend application running on:
- **Port**: 5501
- **Protocol**: HTTPS
- **URL**: `https://localhost:5501`

CORS is configured to allow requests from the frontend.

---

## 📝 API Documentation

- **Swagger UI**: Available at `/swagger`
- **OpenAPI Spec**: Available at `/openapi/v1.json`
- **Complete Feature Guide**: [BACKEND_FEATURES_PROMPT.md](BACKEND_FEATURES_PROMPT.md)
- **Endpoints Reference**: [API_ENDPOINTS.md](API_ENDPOINTS.md)

---

## 🔒 Security

- HTTPS enforced with HSTS
- JWT token authentication
- Token blacklisting on logout
- Rate limiting on all endpoints
- HMAC verification for payment callbacks
- File upload virus scanning
- SQL injection protection (EF Core parameterized queries)
- XSS protection
- CSRF protection

---

## 🚀 Deployment

### Environment Variables (Production)
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=https://localhost:7178
export JWT__Key="your-production-jwt-key"
# ... (see SETUP_SECRETS.md for complete list)
```

### Recommended Hosting
- Azure App Service
- AWS Elastic Beanstalk
- Docker Container
- IIS (Windows Server)

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Authors

- **Your Name** - *Initial work*

---

## 🙏 Acknowledgments

- ASP.NET Core Team
- Paymob for payment gateway
- SendGrid for email service
- Twilio for SMS service
- All open-source contributors

---

## 📞 Support

For issues, questions, or contributions:
- Open an issue on GitHub
- Check the documentation files
- Review the setup guides

---

## 📈 Project Status

🚧 **Active Development** - This project is currently under active development.

---

**Last Updated**: February 16, 2026
