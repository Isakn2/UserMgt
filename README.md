# User Management Web Application

This is a web application built using **ASP.NET Core** and **SQL Server** for managing users. It includes features like user registration, authentication, and an admin panel for managing users. The application ensures data uniqueness with a unique index in the database and provides a responsive UI using **Bootstrap**.

## Features

- **User Registration and Authentication**:
  - Users can register with a unique email and any non-empty password.
  - No email confirmation is required.
  - Blocked users cannot log in, and deleted users can re-register.

- **Admin Panel**:
  - Only authenticated users can access the admin panel.
  - Displays a table of users with the following fields:
    - Selection checkbox
    - Name
    - Email
    - Last login time
    - Status (Active/Blocked)
  - Supports sorting by last login time.

- **Toolbar Actions**:
  - **Block**: Block selected users.
  - **Unblock**: Unblock selected users.
  - **Delete**: Delete selected users.

- **Multiple Selection**:
  - Users can select multiple rows using checkboxes.
  - A "Select All/Deselect All" checkbox is available in the table header.

- **Security**:
  - Before each request (except registration/login), the server checks if the user exists and is not blocked.
  - If the user is blocked or deleted, they are redirected to the login page.

- **Database**:
  - A unique index ensures email uniqueness in the database.
  - The database guarantees email uniqueness even with simultaneous data pushes from multiple sources.

- **UI/UX**:
  - Built using **Bootstrap** for a responsive design.
  - Tooltips and status messages provide feedback to users.
  - No animations, browser alerts, or buttons in data rows.

---

## Technologies Used

- **Backend**: ASP.NET Core
- **Frontend**: Razor Pages, Bootstrap
- **Database**: MySQL
- **Authentication**: ASP.NET Core Identity
- **Deployment**: Remote hosting

---

## Setup Instructions

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or a compatible database
- A code editor (e.g., [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/))


## Database Schema

The database includes the following key tables:

- **Users**:
  - `Id` (Primary Key)
  - `Name`
  - `Email` (Unique Index)
  - `PasswordHash`
  - `LastLoginTime`
  - `Status` (Active/Blocked)
  - `RegistrationTime`


## Error Handling and Feedback

- **Error Messages**: Displayed for invalid inputs or failed operations.
- **Tooltips**: Provide additional information for buttons and actions.
- **Status Messages**: Inform users about successful operations (e.g., "User blocked successfully").

---

## Deployment

The application is deployed and remotely accessible. You can access it at [Deployed Application URL](#).

---

## Contributing

Contributions are welcome! If you find any issues or have suggestions, please open an issue or submit a pull request.

---

## License

This project is licensed under the MIT License.

