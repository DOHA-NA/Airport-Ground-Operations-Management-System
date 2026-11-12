using System;
using System.Collections.Generic;
using System.Linq;

enum FlightTypes
{
    Domestic = 1,
    International = 2
}

enum PassengerCategories
{
    Standard = 1,
    VIP = 2,
    ReducedMobility = 3
}

class Flight
{
    public string FlightNumber { get; set; }
    public FlightTypes FlightType { get; set; }
    public DateTime ArrivalTime { get; set; }
    public DateTime DepartureTime { get; set; }
    public int SeatCapacity { get; set; }
    public int StandbyLimit { get; set; } // FIX 1: standby list capacity per flight
    public string Status { get; set; } = "Scheduled";
    public string GateNumber { get; set; }
}

class Gate
{
    public int IdGate { get; set; }
    public string GateNumber { get; set; }
    public bool InternationalAllowd { get; set; }
    public string AssignedFlightNumber { get; set; }
    public bool IsOccupied { get; set; } = false;
}

class Passenger
{
    public string IDPassenger { get; set; }
    public string NamePassenger { get; set; }
    public PassengerCategories Category { get; set; }

    public string ArrivalFlightNumber { get; set; }
    public string DepartureFlightNumber { get; set; }
}

class Baggage
{
    public string PassengerID { get; set; }
    public string FlightNumber { get; set; }
    public double BaggageWeight { get; set; }
}

class GroundStaff
{
    public string NameStaff { get; set; }
    public int IDStaff { get; set; }
    public string AssignedTask { get; set; }
    public string AssignedFlightNumber { get; set; }
    public string AssignedGateNumber { get; set; }
    public double AssignmentHours { get; set; }
    public double CumulativeHours { get; set; }
    public double MaxShiftHours { get; set; } = 7.0;
    public double RemainingHours
    {
        get { return MaxShiftHours - CumulativeHours; }
    }
}

class Booking
{
    public string PassengerID { get; set; }
    public string FlightNumber { get; set; }
    public string Status { get; set; }
    public DateTime BookingTime { get; set; }

    public Booking(string passengerId, string flightNumber, string status)
    {
        PassengerID = passengerId;
        FlightNumber = flightNumber;
        Status = status;
        BookingTime = DateTime.Now;
    }
}

class Airport
{
    public List<Flight> flights = new List<Flight>();
    public List<Gate> gates = new List<Gate>();
    public List<Passenger> passengers = new List<Passenger>();
    public List<Baggage> baggageList = new List<Baggage>();
    public List<Booking> bookings = new List<Booking>();
    public List<GroundStaff> groundStaff = new List<GroundStaff>();

    // 1. Register Flight
    public void RegisterFlight()
    {
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        if (flights.Any(f => f.FlightNumber.Equals(
            flightNumber, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("RESULT: REJECTED - Flight number already exists.");
            return;
        }

        Console.WriteLine("Select Flight Type:");
        Console.WriteLine("1. Domestic");
        Console.WriteLine("2. International");
        Console.Write("Enter choice: ");

        int choice;
        if (!int.TryParse(Console.ReadLine(), out choice) || (choice != 1 && choice != 2))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid flight type.");
            return;
        }

        FlightTypes flightType = (choice == 1) ? FlightTypes.Domestic : FlightTypes.International;

        Console.Write("Enter Arrival Time: ");
        DateTime arrivalTime;
        if (!DateTime.TryParse(Console.ReadLine(), out arrivalTime))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid arrival time.");
            return;
        }

        Console.Write("Enter Departure Time: ");
        DateTime departureTime;
        if (!DateTime.TryParse(Console.ReadLine(), out departureTime))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid departure time.");
            return;
        }

        Console.Write("Enter Seat Capacity: ");
        int seatCapacity;
        if (!int.TryParse(Console.ReadLine(), out seatCapacity))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid seat capacity.");
            return;
        }

        // FIX 1: ask for standby limit
        Console.Write("Enter Standby List Limit: ");
        int standbyLimit;
        if (!int.TryParse(Console.ReadLine(), out standbyLimit))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid standby limit.");
            return;
        }

        Flight flight = new Flight();

        flight.FlightNumber = flightNumber;
        flight.FlightType = flightType;
        flight.ArrivalTime = arrivalTime;
        flight.DepartureTime = departureTime;
        flight.SeatCapacity = seatCapacity;
        flight.StandbyLimit = standbyLimit;

        flights.Add(flight);

        Console.WriteLine("RESULT: CONFIRMED - Flight registered successfully.");
    }

    // 2. Assign Gate
    public void AssignGate()
    {
        Console.Write("Enter Flight Number: ");
        string flightNum = Console.ReadLine();

        Flight flight = flights.FirstOrDefault(
            f => f.FlightNumber.Equals(flightNum,
            StringComparison.OrdinalIgnoreCase));

        if (flight == null)
        {
            Console.WriteLine("RESULT: REJECTED - Flight not found.");
            return;
        }

        Console.Write("Enter Gate Number: ");
        string gateNum = Console.ReadLine();

        Gate gate = gates.FirstOrDefault(
            g => g.GateNumber.Equals(gateNum,
            StringComparison.OrdinalIgnoreCase));

        if (gate == null)
        {
            Console.WriteLine("RESULT: REJECTED - Gate not found.");
            return;
        }

        if (flight.FlightType == FlightTypes.International &&
            !gate.InternationalAllowd)
        {
            Console.WriteLine("RESULT: REJECTED - Gate does not allow International flights.");
            return;
        }

        bool occupied = flights.Any(f =>
            f.GateNumber != null &&
            f.GateNumber.Equals(gateNum,
            StringComparison.OrdinalIgnoreCase) &&
            f.FlightNumber != flight.FlightNumber &&
            flight.ArrivalTime < f.DepartureTime &&
            flight.DepartureTime > f.ArrivalTime);

        if (occupied)
        {
            Console.WriteLine("RESULT: REJECTED - Gate is occupied during this time.");
            return;
        }

        flight.GateNumber = gateNum;
        gate.AssignedFlightNumber = flightNum;

        Console.WriteLine("RESULT: CONFIRMED - Gate assigned.");
    }

    // 3. Register Passenger
    public void RegisterPassenger()
    {
        Console.Write("Enter Passenger ID: ");
        string id = Console.ReadLine();

        if (passengers.Any(p => p.IDPassenger == id))
        {
            Console.WriteLine("RESULT: REJECTED - Passenger already exists.");
            return;
        }

        Console.Write("Enter Passenger Name: ");
        string name = Console.ReadLine();

        Console.WriteLine("Select Passenger Category:");
        Console.WriteLine("1. Standard");
        Console.WriteLine("2. VIP");
        Console.WriteLine("3. Reduced Mobility");
        Console.Write("Enter choice: ");

        // FIX 3: TryParse instead of Convert.ToInt32
        int choice;
        if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
        {
            Console.WriteLine("RESULT: REJECTED - Invalid category.");
            return;
        }

        PassengerCategories category;

        if (choice == 1)
            category = PassengerCategories.Standard;
        else if (choice == 2)
            category = PassengerCategories.VIP;
        else
            category = PassengerCategories.ReducedMobility;

        Console.Write("Enter Arrival Flight Number (or press Enter if none): ");
        string arrivalFlight = Console.ReadLine();

        Console.Write("Enter Departure Flight Number (or press Enter if none): ");
        string departureFlight = Console.ReadLine();

        Passenger passenger = new Passenger();

        passenger.IDPassenger = id;
        passenger.NamePassenger = name;
        passenger.Category = category;
        passenger.ArrivalFlightNumber = arrivalFlight;
        passenger.DepartureFlightNumber = departureFlight;

        passengers.Add(passenger);

        Console.WriteLine("RESULT: CONFIRMED - Passenger registered.");
    }

    // 4. Check Boarding Eligibility
    public void CheckBoardingEligibility()
    {
        Console.Write("Enter Passenger ID: ");
        string passengerId = Console.ReadLine();

        Passenger passenger = passengers.FirstOrDefault(
            p => p.IDPassenger == passengerId);

        if (passenger == null)
        {
            Console.WriteLine("RESULT: REJECTED - Passenger not found.");
            return;
        }
        Console.Write("Enter Next Flight Number: ");
        string nextFlightNumber = Console.ReadLine();

        if (passenger.ArrivalFlightNumber == "" || string.IsNullOrEmpty(passenger.ArrivalFlightNumber))
        {
            Console.WriteLine("RESULT: CONFIRMED - Passenger is not connecting.");
            return;
        }

        Flight arrivalFlight = flights.FirstOrDefault(f => f.FlightNumber == passenger.ArrivalFlightNumber);

        Flight nextFlight = flights.FirstOrDefault(f => f.FlightNumber == nextFlightNumber);

        if (arrivalFlight == null || nextFlight == null)
        {
            Console.WriteLine("RESULT: REJECTED - Connecting flight not found.");
            return;
        }

        TimeSpan connectionTime =
            nextFlight.DepartureTime - arrivalFlight.ArrivalTime;

        double minutes = connectionTime.TotalMinutes;
        if (minutes < 45)
        {
            Console.WriteLine("RESULT: BOARDING DENIED");
            Console.WriteLine("Reason: Only " + minutes +
                " minutes remain since the connecting flight's arrival; " +
                "the minimum connection time is 45 minutes.");
            return;
        }

        Console.WriteLine("RESULT: BOARDING ALLOWED");
    }

    // 5. Process Boarding
    public void ProcessBoarding()
    {
        Console.Write("Enter Passenger ID: ");
        string passengerId = Console.ReadLine();

        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        Passenger passenger = passengers.FirstOrDefault(
            p => p.IDPassenger == passengerId);

        Flight flight = flights.FirstOrDefault(
            f => f.FlightNumber == flightNumber);

        if (passenger == null || flight == null)
        {
            Console.WriteLine("RESULT: REJECTED - Passenger or flight not found.");
            return;
        }

        if (flight.Status == "Departed" || flight.Status == "Cancelled")
        {
            Console.WriteLine("RESULT: BOARDING DENIED - Flight is not available.");
            return;
        }

        Booking booking = bookings.FirstOrDefault(
            b => b.PassengerID == passengerId &&
            b.FlightNumber == flightNumber &&
            b.Status == "Confirmed");

        if (booking == null)
        {
            Console.WriteLine("RESULT: BOARDING DENIED - Passenger has no confirmed booking.");
            return;
        }

        if (passenger.ArrivalFlightNumber != "")
        {
            Flight arrivalFlight = flights.FirstOrDefault(
                f => f.FlightNumber == passenger.ArrivalFlightNumber);

            if (arrivalFlight != null)
            {
                double minutes =
                    (flight.DepartureTime - arrivalFlight.ArrivalTime).TotalMinutes;

                // FIX 2: specific message with actual numbers
                if (minutes < 45)
                {
                    Console.WriteLine("RESULT: BOARDING DENIED");
                    Console.WriteLine("Reason: Only " + minutes +
                        " minutes remain since the connecting flight's arrival; " +
                        "the minimum connection time is 45 minutes.");
                    return;
                }
            }
        }

        Console.WriteLine("RESULT: BOARDING CONFIRMED");
    }

    // 6. Register Baggage
    public void RegisterBaggage()
    {
        Console.Write("Enter Passenger ID: ");
        string passengerId = Console.ReadLine();

        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        Passenger passenger = passengers.FirstOrDefault(
            p => p.IDPassenger == passengerId);

        Flight flight = flights.FirstOrDefault(
            f => f.FlightNumber == flightNumber);

        if (passenger == null || flight == null)
        {
            Console.WriteLine("RESULT: REJECTED - Passenger or flight not found.");
            return;
        }

        if (flight.Status == "Departed" || flight.Status == "Cancelled")
        {
            Console.WriteLine("RESULT: REJECTED - Baggage cannot be loaded.");
            return;
        }

        Console.Write("Enter Baggage Weight (kg): ");

        // FIX 3: TryParse instead of Convert.ToDouble
        double weight;
        if (!double.TryParse(Console.ReadLine(), out weight))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid baggage weight.");
            return;
        }

        double allowance = 30;

        if (passenger.Category == PassengerCategories.VIP)
            allowance = 40;

        if (passenger.Category == PassengerCategories.ReducedMobility)
            allowance = 35;

        double currentWeight = baggageList
            .Where(b => b.PassengerID == passengerId &&
                        b.FlightNumber == flightNumber)
            .Sum(b => b.BaggageWeight);

        double newTotal = currentWeight + weight;

        // FIX 2: specific message with actual numbers
        if (newTotal > allowance)
        {
            Console.WriteLine("RESULT: BAGGAGE REJECTED");
            Console.WriteLine("Reason: This bag would bring the passenger's total checked baggage to " +
                newTotal + "kg, exceeding the " + allowance +
                "kg allowance for " + passenger.Category + " passengers.");
            return;
        }

        Baggage baggage = new Baggage();

        baggage.PassengerID = passengerId;
        baggage.FlightNumber = flightNumber;
        baggage.BaggageWeight = weight;

        baggageList.Add(baggage);

        Console.WriteLine("RESULT: BAGGAGE CONFIRMED");
        Console.WriteLine("Total baggage weight: " + newTotal + " kg");
    }

    // FIX 4: View a passenger's cumulative baggage weight for a flight
    public void ViewBaggage()
    {
        Console.Write("Enter Passenger ID: ");
        string passengerId = Console.ReadLine();

        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        double total = baggageList
            .Where(b => b.PassengerID == passengerId &&
                        b.FlightNumber == flightNumber)
            .Sum(b => b.BaggageWeight);

        Console.WriteLine("Total Checked Baggage for " + passengerId +
            " on flight " + flightNumber + ": " + total + " kg");
    }

    // 7. Book Passenger
    public void CreateBooking()
    {
        Console.Write("Enter Passenger ID: ");
        string passengerId = Console.ReadLine();

        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        Passenger passenger = passengers.FirstOrDefault(
            p => p.IDPassenger == passengerId);

        Flight flight = flights.FirstOrDefault(
            f => f.FlightNumber == flightNumber);

        if (passenger == null || flight == null)
        {
            Console.WriteLine("RESULT: REJECTED - Passenger or flight not found.");
            return;
        }

        int confirmedCount = bookings.Count(
            b => b.FlightNumber == flightNumber &&
                 b.Status == "Confirmed");

        int standbyCount = bookings.Count(
            b => b.FlightNumber == flightNumber &&
                 b.Status == "Standby");

        string status;

        if (confirmedCount < flight.SeatCapacity)
        {
            status = "Confirmed";
        }
        else if (standbyCount < flight.StandbyLimit)
        {
            // FIX 1: enforce standby list limit
            status = "Standby";
        }
        else
        {
            Console.WriteLine("RESULT: REJECTED - The standby list for this flight has reached its limit.");
            return;
        }

        Booking booking = new Booking(
            passengerId,
            flightNumber,
            status);

        bookings.Add(booking);

        Console.WriteLine("RESULT: " + status);
    }

    // 8. Cancel Booking
    public void CancelBooking()
    {
        Console.Write("Enter Passenger ID: ");
        string passengerId = Console.ReadLine();

        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        Booking booking = bookings.FirstOrDefault(
            b => b.PassengerID == passengerId &&
            b.FlightNumber == flightNumber &&
            b.Status == "Confirmed");

        if (booking == null)
        {
            Console.WriteLine("RESULT: REJECTED - Confirmed booking not found.");
            return;
        }

        booking.Status = "Cancelled";

        Booking standby = bookings.FirstOrDefault(
            b => b.FlightNumber == flightNumber &&
            b.Status == "Standby");

        if (standby != null)
        {
            standby.Status = "Confirmed";
            Console.WriteLine("Earliest standby passenger was promoted.");
        }

        Console.WriteLine("RESULT: BOOKING CANCELLED");
    }

    // 9. View Standby List
    public void ViewStandby()
    {
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        var standbyList = bookings
            .Where(b => b.FlightNumber == flightNumber &&
                        b.Status == "Standby")
            .ToList();

        Console.WriteLine("Standby List:");

        foreach (Booking booking in standbyList)
        {
            Console.WriteLine(booking.PassengerID);
        }
    }

    // 10. Update Flight Status
    public void UpdateFlightStatus()
    {
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        Flight flight = flights.FirstOrDefault( f => f.FlightNumber == flightNumber);

        if (flight == null)
        {
            Console.WriteLine("RESULT: REJECTED - Flight not found.");
            return;
        }

        Console.WriteLine("1. Scheduled");
        Console.WriteLine("2. Delayed");
        Console.WriteLine("3. Boarding");
        Console.WriteLine("4. Departed");
        Console.WriteLine("5. Cancelled");

        Console.Write("Select Status: ");

        // FIX 3: TryParse instead of Convert.ToInt32
        int choice;
        if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 5)
        {
            Console.WriteLine("RESULT: REJECTED - Invalid status choice.");
            return;
        }

        if (choice == 1)
            flight.Status = "Scheduled";
        else if (choice == 2)
            flight.Status = "Delayed";
        else if (choice == 3)
            flight.Status = "Boarding";
        else if (choice == 4)
            flight.Status = "Departed";
        else if (choice == 5)
            flight.Status = "Cancelled";

        Console.WriteLine("RESULT: CONFIRMED - Flight status updated.");
    }

    // 11. Assign Staff
    public void AssignStaff()
    {
        Console.Write("Enter Staff ID: ");

        // FIX 3: TryParse instead of Convert.ToInt32
        int staffId;
        if (!int.TryParse(Console.ReadLine(), out staffId))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid staff ID.");
            return;
        }

        GroundStaff staff = groundStaff.FirstOrDefault(
            s => s.IDStaff == staffId);

        if (staff == null)
        {
            Console.WriteLine("RESULT: REJECTED - Staff member not found.");
            return;
        }

        Console.Write("Enter Task: ");
        string task = Console.ReadLine();

        // FIX 5: link staff to a flight or a gate
        Console.WriteLine("Assign To:");
        Console.WriteLine("1. Flight");
        Console.WriteLine("2. Gate");
        Console.Write("Enter choice: ");

        int assignChoice;
        if (!int.TryParse(Console.ReadLine(), out assignChoice) ||
            (assignChoice != 1 && assignChoice != 2))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid choice.");
            return;
        }

        string assignedFlight = null;
        string assignedGate = null;

        if (assignChoice == 1)
        {
            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine();

            Flight flight = flights.FirstOrDefault(
                f => f.FlightNumber == flightNumber);

            if (flight == null)
            {
                Console.WriteLine("RESULT: REJECTED - Flight not found.");
                return;
            }

            assignedFlight = flightNumber;
        }
        else
        {
            Console.Write("Enter Gate Number: ");
            string gateNumber = Console.ReadLine();

            Gate gate = gates.FirstOrDefault(
                g => g.GateNumber == gateNumber);

            if (gate == null)
            {
                Console.WriteLine("RESULT: REJECTED - Gate not found.");
                return;
            }

            assignedGate = gateNumber;
        }

        Console.Write("Enter Assignment Hours: ");

        // FIX 3: TryParse instead of Convert.ToDouble
        double hours;
        if (!double.TryParse(Console.ReadLine(), out hours))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid hours.");
            return;
        }

        // FIX 2: specific message with actual numbers
        if (staff.CumulativeHours + hours > staff.MaxShiftHours)
        {
            Console.WriteLine("RESULT: REJECTED - This assignment would bring cumulative hours to " +
                (staff.CumulativeHours + hours) + ", exceeding the maximum shift limit of " +
                staff.MaxShiftHours + " hours.");
            return;
        }

        staff.AssignedTask = task;
        staff.AssignedFlightNumber = assignedFlight;
        staff.AssignedGateNumber = assignedGate;
        staff.AssignmentHours = hours;
        staff.CumulativeHours += hours;

        Console.WriteLine("RESULT: CONFIRMED - Staff assigned.");
    }

    // FIX 4: View a staff member's cumulative duty hours
    public void ViewStaffHours()
    {
        Console.Write("Enter Staff ID: ");

        int staffId;
        if (!int.TryParse(Console.ReadLine(), out staffId))
        {
            Console.WriteLine("RESULT: REJECTED - Invalid staff ID.");
            return;
        }

        GroundStaff staff = groundStaff.FirstOrDefault(
            s => s.IDStaff == staffId);

        if (staff == null)
        {
            Console.WriteLine("RESULT: REJECTED - Staff member not found.");
            return;
        }

        Console.WriteLine("Staff Name: " + staff.NameStaff);
        Console.WriteLine("Cumulative Duty Hours: " + staff.CumulativeHours);
        Console.WriteLine("Remaining Hours: " + staff.RemainingHours);
    }

    // Seed Gates
    public void SeedGates()
    {
        gates.Add(new Gate
        {
            IdGate = 1,
            GateNumber = "G1",
            InternationalAllowd = true
        });

        gates.Add(new Gate
        {
            IdGate = 2,
            GateNumber = "G2",
            InternationalAllowd = true
        });

        gates.Add(new Gate
        {
            IdGate = 3,
            GateNumber = "G3",
            InternationalAllowd = false
        });

        gates.Add(new Gate
        {
            IdGate = 4,
            GateNumber = "G4",
            InternationalAllowd = false
        });
    }

    // Main Menu
    public void ShowMenu()
    {
        Console.WriteLine("\n=== Meridian Terminal — Ground Operations System ===");
        Console.WriteLine("1. Register Flight");
        Console.WriteLine("2. Assign Gate");
        Console.WriteLine("3. Register Passenger");
        Console.WriteLine("4. Check Boarding Eligibility");
        Console.WriteLine("5. Register Baggage");
        Console.WriteLine("6. Manage Bookings & Standby");
        Console.WriteLine("7. Assign Staff");
        Console.WriteLine("8. Update Flight Status");
        Console.WriteLine("9. Process Boarding");
        Console.WriteLine("10. Exit");
    }

    static void Main(string[] args)
    {
        Airport airport = new Airport();

        airport.SeedGates();

        bool running = true;

        while (running)
        {
            airport.ShowMenu();

            Console.Write("Select an option: ");

            // FIX 3: TryParse instead of Convert.ToInt32
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            if (choice == 1)
                airport.RegisterFlight();

            else if (choice == 2)
                airport.AssignGate();

            else if (choice == 3)
                airport.RegisterPassenger();

            else if (choice == 4)
                airport.CheckBoardingEligibility();

            else if (choice == 5)
                airport.RegisterBaggage();

            else if (choice == 6)
            {
                Console.WriteLine("1. Book Passenger");
                Console.WriteLine("2. Cancel Booking");
                Console.WriteLine("3. View Standby List");
                Console.WriteLine("4. View Passenger Baggage Weight");

                Console.Write("Select: ");

                int bookingChoice;
                if (!int.TryParse(Console.ReadLine(), out bookingChoice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                if (bookingChoice == 1)
                    airport.CreateBooking();
                else if (bookingChoice == 2)
                    airport.CancelBooking();
                else if (bookingChoice == 3)
                    airport.ViewStandby();
                else if (bookingChoice == 4)
                    airport.ViewBaggage();
            }

            else if (choice == 7)
            {
                Console.WriteLine("1. Assign Staff");
                Console.WriteLine("2. View Staff Duty Hours");

                Console.Write("Select: ");

                int staffMenuChoice;
                if (!int.TryParse(Console.ReadLine(), out staffMenuChoice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                if (staffMenuChoice == 1)
                    airport.AssignStaff();
                else if (staffMenuChoice == 2)
                    airport.ViewStaffHours();
            }

            else if (choice == 8)
                airport.UpdateFlightStatus();

            else if (choice == 9)
                airport.ProcessBoarding();

            else if (choice == 10)
                running = false;

            else
                Console.WriteLine("Invalid option.");
        }

        Console.WriteLine("Session ended.");
    }
}