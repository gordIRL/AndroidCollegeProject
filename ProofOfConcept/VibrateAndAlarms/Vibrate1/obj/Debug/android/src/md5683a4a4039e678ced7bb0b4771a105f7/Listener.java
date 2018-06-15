package md5683a4a4039e678ced7bb0b4771a105f7;


public class Listener
	extends android.view.OrientationEventListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onOrientationChanged:(I)V:GetOnOrientationChanged_IHandler\n" +
			"";
		mono.android.Runtime.register ("Xamarin.Essentials.Listener, Xamarin.Essentials, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Listener.class, __md_methods);
	}


	public Listener (android.content.Context p0)
	{
		super (p0);
		if (getClass () == Listener.class)
			mono.android.TypeManager.Activate ("Xamarin.Essentials.Listener, Xamarin.Essentials, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public Listener (android.content.Context p0, int p1)
	{
		super (p0, p1);
		if (getClass () == Listener.class)
			mono.android.TypeManager.Activate ("Xamarin.Essentials.Listener, Xamarin.Essentials, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Hardware.SensorDelay, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public void onOrientationChanged (int p0)
	{
		n_onOrientationChanged (p0);
	}

	private native void n_onOrientationChanged (int p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
