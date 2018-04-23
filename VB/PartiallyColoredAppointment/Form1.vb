Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler
Imports DevExpress.XtraScheduler.Drawing
Imports DevExpress.Utils.Drawing

Namespace SchedulerProject
	Partial Public Class Form1
		Inherits Form

		#Region "InitialDataConstants"
		Public Shared RandomInstance As New Random()
		Public Shared Users() As String = {"Peter Dolan", "Ryan Fischer", "Richard Fisher", "Tom Hamlett", "Mark Hamilton", "Steve Lee", "Jimmy Lewis", "Jeffrey W McClain", "Andrew Miller", "Dave Murrel", "Bert Parkins", "Mike Roller", "Ray Shipman", "Paul Bailey", "Brad Barnes", "Carl Lucas", "Jerry Campbell"}
		#End Region

		Public Sub New()
			InitializeComponent()
			FillResources(schedulerStorage1, 5)
			InitAppointments()
			schedulerControl1.Start = DateTime.Now
			UpdateControls()

		End Sub

		#Region "InitialDataLoad"
		Private Sub FillResources(ByVal storage As SchedulerStorage, ByVal count As Integer)
			Dim resources As ResourceCollection = storage.Resources.Items
			storage.BeginUpdate()
			Try
				Dim cnt As Integer = Math.Min(count, Users.Length)
				For i As Integer = 1 To cnt
					resources.Add(New Resource(i, Users(i - 1)))
				Next i
			Finally
				storage.EndUpdate()
			End Try
		End Sub
		Private Sub InitAppointments()
			Me.schedulerStorage1.Appointments.Mappings.Start = "StartTime"
			Me.schedulerStorage1.Appointments.Mappings.End = "EndTime"
			Me.schedulerStorage1.Appointments.Mappings.Subject = "Subject"
			Me.schedulerStorage1.Appointments.Mappings.AllDay = "AllDay"
			Me.schedulerStorage1.Appointments.Mappings.Description = "Description"
			Me.schedulerStorage1.Appointments.Mappings.Label = "Label"
			Me.schedulerStorage1.Appointments.Mappings.Location = "Location"
			Me.schedulerStorage1.Appointments.Mappings.RecurrenceInfo = "RecurrenceInfo"
			Me.schedulerStorage1.Appointments.Mappings.ReminderInfo = "ReminderInfo"
			Me.schedulerStorage1.Appointments.Mappings.ResourceId = "OwnerId"
			Me.schedulerStorage1.Appointments.Mappings.Status = "Status"
			Me.schedulerStorage1.Appointments.Mappings.Type = "EventType"

			Dim eventList As New CustomEventList()
			GenerateEvents(eventList)
			Me.schedulerStorage1.Appointments.DataSource = eventList

		End Sub
		Private Sub GenerateEvents(ByVal eventList As CustomEventList)
			Dim count As Integer = schedulerStorage1.Resources.Count
			For i As Integer = 0 To count - 1
				Dim resource As Resource = schedulerStorage1.Resources(i)
				Dim subjPrefix As String = resource.Caption & "'s "
				eventList.Add(CreateEvent(eventList, subjPrefix & "meeting", resource.Id, 2, 5))
				eventList.Add(CreateEvent(eventList, subjPrefix & "travel", resource.Id, 3, 6))
				eventList.Add(CreateEvent(eventList, subjPrefix & "phone call", resource.Id, 0, 10))
			Next i
		End Sub
		Private Function CreateEvent(ByVal eventList As CustomEventList, ByVal subject As String, ByVal resourceId As Object, ByVal status As Integer, ByVal label As Integer) As CustomEvent
			Dim apt As New CustomEvent(eventList)
			apt.Subject = subject
			apt.OwnerId = resourceId
			Dim rnd As Random = RandomInstance
			Dim rangeInMinutes As Integer = 60 * 24
			apt.StartTime = DateTime.Today + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes))
			apt.EndTime = apt.StartTime + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes \ 4))
			apt.Status = status
			apt.Label = label
			Return apt
		End Function
		#End Region
		#Region "Update Controls"
		Private Sub UpdateControls()
			cbView.EditValue = schedulerControl1.ActiveViewType
			rgrpGrouping.EditValue = schedulerControl1.GroupType
		End Sub
		Private Sub rgrpGrouping_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rgrpGrouping.SelectedIndexChanged
			schedulerControl1.GroupType = CType(rgrpGrouping.EditValue, SchedulerGroupType)
		End Sub

		Private Sub schedulerControl_ActiveViewChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles schedulerControl1.ActiveViewChanged
			cbView.EditValue = schedulerControl1.ActiveViewType
		End Sub

		Private Sub cbView_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbView.SelectedIndexChanged
			schedulerControl1.ActiveViewType = CType(cbView.EditValue, SchedulerViewType)
		End Sub
		#End Region

		Private Sub schedulerControl1_CustomDrawAppointmentBackground(ByVal sender As Object, ByVal e As CustomDrawObjectEventArgs) Handles schedulerControl1.CustomDrawAppointmentBackground
			' Specify the task completed ratio
			Dim completed As Double = 0.25

			Dim bounds As Rectangle = CalculateEntireAppointmentBounds(CType(e.ObjectInfo, AppointmentViewInfo))
			DrawBackGroundCore(e.Cache, bounds, completed)
			e.Handled = True

		End Sub

		Private Function CalculateEntireAppointmentBounds(ByVal viewInfo As AppointmentViewInfo) As Rectangle
			Dim leftOffset As Integer = 0
			Dim rightOffset As Integer = 0
			Dim scale As Double = viewInfo.Bounds.Width / viewInfo.Interval.Duration.TotalMilliseconds
			If (Not viewInfo.HasLeftBorder) Then
				Dim hidden As Double = (viewInfo.Interval.Start - viewInfo.AppointmentInterval.Start).TotalMilliseconds
				leftOffset = CInt(Fix(hidden * scale))
			End If
			If (Not viewInfo.HasRightBorder) Then
				Dim hidden As Double = (viewInfo.AppointmentInterval.End - viewInfo.Interval.End).TotalMilliseconds
				rightOffset = CInt(Fix(hidden * scale))
			End If
			Dim bounds As Rectangle = viewInfo.Bounds
			Return Rectangle.FromLTRB(bounds.Left - leftOffset, bounds.Y, bounds.Right + rightOffset, bounds.Bottom)
		End Function
		Private Sub DrawBackGroundCore(ByVal cache As GraphicsCache, ByVal bounds As Rectangle, ByVal completed As Double)
			Dim brush1 As Brush = New SolidBrush(Color.Green)
			Dim brush2 As Brush = New SolidBrush(Color.Orange)
			cache.FillRectangle(brush1, New Rectangle(bounds.X, bounds.Y, CInt(Fix(bounds.Width * completed)), bounds.Height))
			cache.FillRectangle(brush2, New Rectangle(bounds.X + CInt(Fix(bounds.Width * completed)), bounds.Y, CInt(Fix(bounds.Width * (1 - completed))), bounds.Height))
		End Sub

	End Class
End Namespace