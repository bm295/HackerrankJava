static boolean isAnagram(String a, String b) {
    // Complete the function
    a = a.toLowerCase();
    b = b.toLowerCase();
    Map<Character, Integer> dicA = new HashMap<Character, Integer>();
    Map<Character, Integer> dicB = new HashMap<Character, Integer>();
    for(char c : a.toCharArray()) {        
        if (dicA.get(c) == null) {
            dicA.put(c, 1);
        }
        else {
            int val = dicA.get(c);
            dicA.put(c, ++val);
        }
    }
    for(char c : b.toCharArray()) {        
        if (dicB.get(c) == null) {
            dicB.put(c, 1);
        }
        else {
            int val = dicB.get(c);
            dicB.put(c, ++val);
        }
    }
    if (dicA.size() != dicB.size()) {
        return false;
    }
    for (Map.Entry<Character, Integer> eA : dicA.entrySet()) {
        if (dicB.get(eA.getKey()) == null || dicB.get(eA.getKey()) != eA.getValue()) {
            return false;
        }
    }
    return true;
}
