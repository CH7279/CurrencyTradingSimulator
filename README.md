# Currency Trading Simulator

This project simulates real-time currency trading updates using .NET Core, MVC, and an **In-Memory Database** (instead of SQL Server).

## Description

This application simulates the trading of different currency pairs and updates their values every 2 seconds. The system uses a background service to randomly update the trading values and ensures that the minimum and maximum values for each pair are maintained.

### Technologies:
- **.NET Core**
- **MVC**
- **In-Memory Database** (simulated using Entity Framework Core)

### Features:
- Real-time updates of currency rates (every 2 seconds).
- Simulation of currency trading with random fluctuations.
- Minimum and maximum values for currency pairs are dynamically updated.
- Real-time display of updates in a web-based dashboard.

## Setup

### Prerequisites

1. **.NET Core SDK**: Ensure that you have the latest .NET Core SDK installed on your machine.
   - You can download it from [here](https://dotnet.microsoft.com/download/dotnet).

2. **Visual Studio or Visual Studio Code**: 
   - Visual Studio: [Download Visual Studio](https://visualstudio.microsoft.com/)
   - Visual Studio Code: [Download Visual Studio Code](https://code.visualstudio.com/)

3. **Git**: Ensure that Git is installed for version control.
   - You can download Git from [here](https://git-scm.com/).

### Running the Project

1. **Clone the repository**:
   ```bash
   git clone https://github.com/CH7279/CurrencyTradingSimulator.git

2. **Navigate to the project folder**:
   ```bash
   cd CurrencyTradingSimulator

3. **Restore dependencies**:
   - Run the following command to restore the necessary packages:
   ```bash
   dotnet restore
   
4. **Run the application**:
   - After restoring the dependencies, you can run the application using the following command:
   ```bash
   dotnet run
