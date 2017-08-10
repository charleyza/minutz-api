﻿using minutz_interface.Entities;

namespace minutz_models.Entities
{
	public class Instance : IInstance
	{
		public int Id { get ; set; }
		public string Name { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool Active { get; set; }
		public int Type { get; set; }
	}
}