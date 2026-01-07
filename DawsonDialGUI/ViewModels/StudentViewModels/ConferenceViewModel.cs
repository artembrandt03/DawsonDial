using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;

namespace DawsonDialGUI.ViewModels.StudentViewModels
{
    public class ConferenceViewModel
    {
        private readonly Conference _conference;
        private readonly Student _student;
        private readonly Func<Conference, Task> _unregisterFromEvent;
        private readonly Func<Conference, Task> _registerForEvent;

        public ICommand UnregisterCommand { get; }
        public ICommand RegisterCommand { get; }

        public ConferenceViewModel(Conference conference, Student student, Func<Conference, Task> unregisterFromEvent, Func<Conference, Task> registerForEvent)
        {
            _conference = conference ?? throw new ArgumentNullException(nameof(conference));
            _student = student ?? throw new ArgumentNullException(nameof(student));
            _unregisterFromEvent = unregisterFromEvent ?? throw new ArgumentNullException(nameof(unregisterFromEvent));
            _registerForEvent = registerForEvent ?? throw new ArgumentNullException(nameof(registerForEvent));

            UnregisterCommand = new AsyncRelayCommand(Unregister);
            RegisterCommand = new AsyncRelayCommand(Register);
        }

        // Pass-through properties
        public string Title => _conference.Title;
        public DateTime StartDateTime => _conference.StartDateTime;
        public DateTime EndDateTime => _conference.EndDateTime;
        public string RoomNumber => _conference.Room.RoomNumber;
        public string ParticipantCount => $"{_conference.Participants.Count}/{_conference.Room.NumberOfSeats}";

        // Helper properties
        public string TypeName => "Conference";
        public string DateText => StartDateTime.ToString("MMM d, yyyy");
        public string TimeText => $"{StartDateTime:h:mm tt} - {EndDateTime:h:mm tt}";

        public DateTime SortableDate => StartDateTime.Date;
        public TimeSpan SortableTime => StartDateTime.TimeOfDay;

        // Registration properties
        public bool IsRegistered => _conference.HasParticipant(_student);
        public bool CanRegister => !IsRegistered && !_conference.IsFull;

        private async Task Unregister()
        {
            await _unregisterFromEvent(_conference);
        }

        private async Task Register()
        {
            await _registerForEvent(_conference);
        }
    }
}
