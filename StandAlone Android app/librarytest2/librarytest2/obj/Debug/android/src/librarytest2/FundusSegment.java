package librarytest2;


public class FundusSegment
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("librarytest2.FundusSegment, librarytest2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", FundusSegment.class, __md_methods);
	}


	public FundusSegment () throws java.lang.Throwable
	{
		super ();
		if (getClass () == FundusSegment.class)
			mono.android.TypeManager.Activate ("librarytest2.FundusSegment, librarytest2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	java.util.ArrayList refList;
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
